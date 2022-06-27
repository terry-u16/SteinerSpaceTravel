# 使い方

## 実行環境

[.NET Runtime 6](https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0)が必要です。事前にインストールをお願いします。

[リンク先](https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0)の ".NET Runtime 6.x.x" セクションから、ご自身の環境に合ったインストーラをダウンロード・実行ください。

### Windows

どれを選ぶべきかよく分からない……という場合は、x86版インストーラをダウンロードすればとりあえず問題ないはずです。

### macOS

Intel macの場合はx64版インストーラを、M1 Mac以降の場合はArm64版インストーラをダウンロードください。

### Linux

[パッケージマネージャーの手順](https://docs.microsoft.com/ja-jp/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website)をクリックし、ご自身のディストリビューションに沿った手順でインストールください。例えばUbuntu 20.04の場合、以下のコマンドでインストールが可能です。

```bash
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-6.0
```

## テストケース生成

`input` ディレクトリに、seed 0～99 に対する入力ファイルが予め用意されています。

より多くの入力ファイルが欲しい場合は、 `seeds.txt` にseed値（64bit符号なし整数）を入力し、以下のコマンドを実行します。

```bash
dotnet tester.dll gen -s seeds.txt
```

## 得点計算・ビジュアライズ

得点計算を行うには、入力ファイル名を `in.txt` 、出力ファイル名を `out.txt` として、以下のコマンドを実行します。

```bash
dotnet tester.dll judge -i in.txt -o out.txt
```

ビジュアライズを行いたい場合は、出力先の画像ファイル名を `vis.png` として、以下のコマンドを実行します。

```bash
dotnet tester.dll judge -i in.txt -o out.txt -v vis.png
```

## 複数ケース並列実行・集計

複数ケースの実行を行いたい場合は、入力ファイルのあるディレクトリを `in` 、解答プログラムの実行コマンドを `cmd` 、同時並列実行数を `parallel` として、以下のコマンドを実行します。このコマンドにより、 `in` ディレクトリ内の全ての `.txt` ファイルに対して解答プログラムが実行され、そのスコアが集計されます。

```bash
dotnet tester.dll judge-all -i in -c "cmd" -p parallel
```

例えば、 `inputs` ディレクトリ内の全てのテストケースを対象として `answer.py` というPythonプログラムを4並列で実行したい場合、以下のコマンドを実行します。

```bash
dotnet tester.dll judge-all -i inputs -c "python answer.py" -p 4
```

ダブルクォーテーション (`""`) を忘れると動作しない場合がありますのでご注意ください。

## その他

- `judge-all` コマンドでは実行時間も表示されますが、実際の時間より若干長くなることがあります。あくまで参考値としてお使いください。
- 途中でコマンドの実行を止めたい場合は、 `ctrl + C` を入力してください。
- 本ツールとジャッジプログラムの動作結果の同一性については保証しません。
- 本ツールを利用した結果生じた損害について、一切の責任を負いません。
- 不明点・不具合等がありましたら、 [@terry_u16](https://twitter.com/terry_u16) までお問い合わせください。
