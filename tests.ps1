$loc = Get-Location

$loc


dotnet test ./K8s.Resource.Configuration.Validator.sln  /p:CollectCoverage=true /p:Exclude="[xunit.*.*]*" /p:CoverletOutput=$loc/testresults/ /p:MergeWith=$loc/testresults/coverage.json /p:CoverletOutputFormat="opencover%2ccobertura%2cjson" --logger=trx --results-directory $loc/testresults/results;

reportgenerator -reports:./testresults/coverage.cobertura.xml -targetdir:./testresults/coverage-reports -reporttypes:Html



# dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura


# reportgenerator
# -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml"
# -targetdir:"coveragereport"
# -reporttypes:Html