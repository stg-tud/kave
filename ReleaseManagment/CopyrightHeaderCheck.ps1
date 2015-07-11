####################### Configuration ##########################

$validYears = "2013","2014","2015","2013-2015"
$validNames = "Technische Universität Darmstadt"

# Specify Files/Folders to exclude (compared to full filepath)
$excludeFiles = "*.generated.cs","*.designer.cs","*TinyMessenger.cs"
$excludeFolders = "*\obj\*","*\bin\*","*\packages\*","*\Packages\*","*\test\data\*" 

####################### Implementation #########################

function Get-ContentAsString ($file)
{
  return (Get-Content $file) -join "`n"
}

filter Where-NotMatch([String[]]$Like) 
{
   if ($Like.Length) {
        foreach ($Pattern in $Like) {
            if ($_ -like $Pattern) { return }
        }
    }
  return $_
}

function Write-InvalidCopyrightHeader($message,$expected,$actual)
{
  Write-Warning $message
  Write-Warning "Expected `n $expected"
  Write-Warning "But was: `n $actual `n"
}    

function IsInValidCopyrightString ($copyright)
{   
    $indexOfYear = $copyright.indexof(" ") + 1
    $indexOfName = $copyright.indexof(" ",$indexofYear) + 1
    $year = $copyright.substring($indexOfYear,$indexOfName-$indexOfYear-1)
    $name = $copyright.substring($indexOfName)
    $isInvalidYear = !($validYears -contains $year)
    $isInvalidName = !($validNames -contains $name)

    return ($isInvalidYear -or $isInvalidName)
}

function ReplaceCopyrightStringInHeader ($header,$copyrightString) 
{
    $indexOfCopyright = $header.IndexOf("Copyright")
    $headerCopyrightString = ($header.substring($indexOfCopyright)).split("`n")[0]

    $header = $header.replace($headerCopyrightString,"")
    $header = $header.insert($indexOfCopyright,$copyrightString)

    return $header    
}

# Returns true if Header is invalid
function Verify-Header ($file,$header)
{
    $fileName = $file.substring($file.IndexOf("feedback-generator") + 19)
    $content = Get-ContentAsString $file
    
    # Split expected file header by line breaks
    $headerLineSplit = $header.split("`n")
    
    # First check whether file starts with a comment
    if(!$content.startswith($header.split("`n")[0])) 
    {
        Write-InvalidCopyrightHeader "$fileName does not contain License-Header" $header " "
        return $TRUE
    }
    
    # Compare contents of existing File Header 
            
    # Retrieve File Header from File 
    $fileHeader = $content.substring($content.IndexOf($headerLineSplit[0]), `    $content.IndexOf($headerLineSplit[-1]) + $headerLineSplit[-1].length)
    $fileHeaderLineSplit = $fileHeader.split("`n")
    
    # Compare File Header of file with expected file header
    $isNotEqualHeader = !$fileHeader.equals($header)
    
    # Check whether File Header has correct length compared to actual File Header
    $hasIncorrectLength = ($headerLineSplit.length -ne  $fileHeaderLineSplit.length)    
    
    # Retrieve Copyright String (e.g. Copyright 2014 Technische Universität Darmstadt) 
    $copyrightString = ($content.substring($content.IndexOf("Copyright"))).split("`n")[0]
    
    # Check whether Copyright String is invalid
    $isInvalidCopyrightString = IsInvalidCopyrightString($CopyrightString)
            
    if($isNotEqualHeader)
    {   
        if($isInvalidCopyrightString -or $hasIncorrectLength)
        {
            $replacedHeader = ReplaceCopyrightStringInHeader $header $copyrightString
            Write-InvalidCopyrightHeader "$fileName has incorrect License-Header" $replacedHeader $fileHeader
            return $TRUE
        }
    }  

    return $FALSE
}

$license = Get-Item ..\feedback-generator\KaVE.licenseheader
$header = Get-ContentAsString $license
$cSharpLicenseHeader = $header.substring($header.IndexOf("/*"),$header.LastIndexOf("*/") - $header.IndexOf("/*") + 2)
$xamlLicenseHeader = $header.substring($header.IndexOf("<!--"),$header.LastIndexOf("-->") - $header.IndexOf("<!--") + 3)

$robocopyArguments = "/L","/S","/FP","/NDL","/NS","/NC","/NJS","/NJH"
$robocopySource = "..\feedback-generator"

# Verify Copyright Header for all C# files (excludes files from obj, bin and package folder)
$resultArray = robocopy $robocopySource  NULL $robocopyArguments `
| ?{$_ -like "*.cs" } | Where-NotMatch $excludeFolders | Where-NotMatch $excludeFiles `
| % {$_.Trim() } | % {Verify-Header $_ $cSharpLicenseHeader } 

# Verify Copyright Header for all xaml/xml files
$resultArray += robocopy $robocopySource NULL $robocopyArguments `
| ?{$_ -like "*.xml" -or $_ -like "*.xaml"} | Where-NotMatch $excludeFolders | Where-NotMatch $excludeFiles `
| % {$_.Trim() } | % {Verify-Header $_ $xamlLicenseHeader } 

$hasError = $resultArray -contains $TRUE

if(!$hasError) 
{
    Write-Host "All files contain the License Header"
}
else 
{
    Write-Error "One or more files contain no or invalid license header"
}