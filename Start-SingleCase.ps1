param(
    [Parameter(mandatory)]
    [int]
    $seed
)


Push-Location .\solver
$inputPath = "..\in\{0:0000}.txt" -f $seed
Get-Content $inputPath | cargo run --release > ../out.txt
Pop-Location
$inputPath = ".\in\{0:0000}.txt" -f $seed
dotnet run -c Release --project .\SteinerSpaceTravel.Console\SteinerSpaceTravel.Console.csproj -- judge -i $inputPath -o out.txt -v vis.png