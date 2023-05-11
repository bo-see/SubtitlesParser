$Script:EpisodesLocation = "C:\LocalDocuments\FHL\Episodes"
$Script:FfmpegFile = ".\ffmpeg.exe"
$Script:SqlConnectionString = "Server=DESKTOP-BJGUGNB\SQLExpress01;AttachDbFilename='C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\AwesomoTest.mdf';Database=AwesomoTest;Trusted_Connection=Yes;"

function Start-SubtitlesParser()
{
    # Check the validity of the file path first
    if(-not (Test-Path -Path $Script:EpisodesLocation))
    {
        Write-Error "$Script:EpisodesLocation is not a valid path."
        return
    }

    # File path exist. Now iterate through all episodes and process the files
    $getChildString = $Script:EpisodesLocation + '\*.mkv'
    
    $allEpisodes = Get-ChildItem -Path $getChildString
    Write-Output "There are $($allEpisodes.Count) episodes in the folder. Iterating now."

    $sqlConnection = New-Object System.Data.SqlClient.SqlConnection 
    $sqlConnection.ConnectionString = $Script:SqlConnectionString
    $sqlConnection.Open()

    try{
        foreach ($episode in $allEpisodes)
        {
            Invoke-EpisodeParser -Episode $episode -SqlConnection $sqlConnection
        }
    }
    catch
    {
        Write-Error "Error updating database with $($_.Exception)"
        $Script:SqlConnection.Close()
    }

    $Script:SqlConnection.Close()
}

function Invoke-EpisodeParser()
{
    param(
        [Object] $Episode,
        [System.Data.SqlClient.SqlConnection] $SqlConnection
    )

    # Parse file name to get episode number
    $regexString = "- S(?<season>\d+)E(?<episodeNumber>\d+) -"
    $Episode.name -match $regexString | Out-Null
    $season = $matches.season
    $episodeNumber = $matches.episodeNumber
    Write-Output "Processing episode: $($Episode.Name): Season $season, episode $episodeNumber."

    # Run FFMPEG to get srt file
    $fullFilePath = Join-Path $Script:EpisodesLocation $Episode.name
    
    Upload-MkvFile -MkvFile $fullFilePath -Season $season -EpisodeNumber $EpisodeNumber -SqlConnection $SqlConnection
}

# Returns an array of row of things to upload into the DB
function Upload-MkvFile()
{
    param(
        [Object] $MkvFile,
        [int] $Season,
        [int] $EpisodeNumber,
        [System.Data.SqlClient.SqlConnection] $SqlConnection
    )

    $ffmpegOutput = & $Script:FfmpegFile -i $MkvFile -map 0:s:0 -y outputFile.srt 2>&1 #TODO: make this into a var 

    # Do the frames calculation
    $line = $ffmpegOutput -match "Stream.*Video.*(?<fps>[0-9\.]) fps"
    $line[0].ToString() -match "Stream.*Video.* (?<fps>[0-9\.]+) fps" | Out-Null
    $fps = $matches.fps

    # Grab the raw content from the SRT File
    $text = Get-Content outputFile.srt -Encoding UTF8
	$lines = ($text -join "`n") -split "`n`n" #entries split by double newline
    
    $test = 0
	foreach($line in $lines)
	{
		$entry = $line -split "`n"
		$timestamps = $entry[1] -split " --> "
		$startTimestamp = $timestamps[0] -replace ',','.'
		$endTimestamp = $timestamps[1] -replace ',','.'
		$text = $entry[2..($entry.length -1)] -Join "`n"

        $startSeconds = ([timespan]$startTimestamp).TotalSeconds
        $endSeconds  = ([timespan]$endTimestamp).TotalSeconds
        $startFrame = [int][Math]::Ceiling($startSeconds * $fps)
        $endFrame =  [int][Math]::Floor($endSeconds * $fps)

        Write-Output "season: $season, episode: $EpisodeNumber, starttimestamp: $startTimestamp, endtimestamp: $endTimestamp, startframe: $startFrame, endFrame: $endFrame, text: $text"
        $sqlLine = "INSERT INTO Captions (Season, EpisodeNumber, StartTimestamp, EndTimestamp, StartFrame, EndFrame, Text) VALUES (@Season, @EpisodeNumber, @StartTimestamp, @EndTimestamp, @StartFrame, @EndFrame, @Text)"
        $command = New-Object System.Data.SqlClient.SqlCommand
        $command.CommandText = $sqlLine
        $command.Connection = $SqlConnection
        $command.Parameters.Add("@Season", $Season) | Out-Null
        $command.Parameters.Add("@EpisodeNumber", $EpisodeNumber) | Out-Null
        $command.Parameters.Add("@StartTimestamp", $startTimestamp) | Out-Null
        $command.Parameters.Add("@EndTimestamp", $endTimestamp) | Out-Null
        $command.Parameters.Add("@StartFrame", $startFrame) | Out-Null
        $command.Parameters.Add("@EndFrame", $endFrame) | Out-Null
        $command.Parameters.Add("@Text", $text) | Out-Null
        $command.ExecuteNonQuery()
        
        if($test++ -gt 3)
        {
            return
        }
    }
}
