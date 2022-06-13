param(
    [Parameter(mandatory)]
    [int]
    $seed
)


Push-Location .\solver
$inputPath = "..\tools\in\{0:0000}.txt" -f $seed
Get-Content $inputPath | cargo run --release > ../out.txt
Pop-Location
python .\vis_output.py -s $seed -o .\out.txt