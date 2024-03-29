# Steiner Space Travel

実行時間制限は **1秒** となっております。ご注意ください。

## ストーリー

天体観測技術の進歩により、未知の惑星が多数発見された。そこで新たに開発された無人探査船を用いて、これらの惑星を全て調査することとなった。惑星間の移動には距離の2乗に比例したエネルギーを消費する。中継点となる宇宙ステーションを適切な場所に配置し、できるだけエネルギー消費量の少ない調査経路を策定してほしい。

## 問題文

惑星の座標が $N$ 個与えられる。各惑星は二次元平面上に存在し、 $i$ 番目の惑星の座標は $(a_i, b_i)$ である。また宇宙ステーションが $M$ 個あり、 $j$ 番目の宇宙ステーションは $0 \le c_j \le 1000,\ 0 \le d_j \le 1000$ を満たす任意の整数座標 $(c_j, d_j)$ に配置することができる。一度配置した宇宙ステーションを再度別の座標に配置し直すことはできない。

惑星と宇宙ステーションを合わせて「経由地」と呼称し、訪問する経由地の数を $V$ 、 $k$ 番目に訪問する経由地の座標を $(x_k, y_k)$ とおく。 $k$ 番目の経由地から $k+1$ 番目の経由地に移動する際に消費するエネルギー $E_k\ (1\le k\le V-1)$ を、経由地間のユークリッド距離 $D_k=\sqrt{(x_k - x_{k+1})^2+(y_k - y_{k+1})^2}$ および定数 $\alpha\ (1\le \alpha)$ を用いて以下のように定める。

- $k$ 番目の経由地と $k+1$ 番目の経由地がともに惑星の場合: $E_k=\alpha^2 D_k^2$
- $k$ 番目の経由地と $k+1$ 番目の経由地のいずれか一方のみが惑星の場合: $E_k=\alpha D_k^2$
- $k$ 番目の経由地と $k+1$ 番目の経由地がともに宇宙ステーションの場合: $E_k=D_k^2$

いずれも消費エネルギーが **ユークリッド距離の2乗に比例** することに注意せよ。

各宇宙ステーションを配置した上で、惑星1から出発して全ての惑星を1回以上訪問し再度惑星1に戻ってくる経路であって消費エネルギーの総和 $\sum_{k=1}^{V-1} E_k$ ができるだけ小さいものを求めよ。同じ経由地を何度訪問してもよく、訪問しない宇宙ステーションが存在しても構わない。また配置した宇宙ステーションの座標が他の経由地と一致していてもよい。

## 得点

消費エネルギーの総和が $S$ であるとき、 $\mathrm{round}(10^9/(1000+\sqrt S))$ 点が得られる。ただし、出力が不正である場合は`WA`と判定される。不正な出力とは以下を指す。

- 出力のフォーマットが正しくない出力
- 変数の値が制約を満たさない出力
- 経路の始点が惑星1でない出力
- 経路の終点が惑星1でない出力
- 未訪問の惑星が存在する出力

テストケースは全部で30個あり、全テストケースの得点の合計が提出の得点となる。コンテスト時間中に得た最高得点で最終順位が決定され、コンテスト終了後のシステムテストは行われない。

## 入力

全てのテストケースにおいて、 $α=5$ で固定である。

入力は以下の形式で標準入力から与えられる。

```text
$N\ M$
$a_1\ b_1$
$\vdots$
$a_N\ b_N$
```

各変数は以下の制約を満たす。

- $N=100$
- $M=8$
- $0\le a_i\le 1000$
- $0\le b_i\le 1000$
- $i\ne i'$ ならば $(a_i, b_i)\ne(a_{i'}, b_{i'})$
- 入力は全て整数

## 出力

以下のフォーマットで標準出力に出力し、最後に改行せよ。

```text
$c_1\ d_1$
$\vdots$
$c_M\ d_M$
$V$
$t_1\ r_1$
$\vdots$
$t_V\ r_V$
```

ここで $t_k$ は $k$ 番目の経由地の種類を表す整数であり、惑星の場合は`1`、宇宙ステーションの場合は`2`である。また $r_k$ は $k$ 番目の経由地の番号であり、惑星の場合は $1\le r_k\le N$ 、宇宙ステーションの場合は $1\le r_k\le M$ である。各変数は以下の制約を満たさなければならない。

- $0\le c_j\le 1000$
- $0\le d_j\le 1000$
- $1\le V\le 10^5$
- $t_k\in \{ 1, 2 \}$
- $t_k=1$ のとき、 $1\le r_k\le N$
- $t_k=2$ のとき、 $1\le r_k\le M$
- $t_1 = t_V = 1$
- $r_1 = r_V = 1$
- 出力は全て整数

## 入力生成方法

この項目は問題の理解に必須ではない。

以下の手順で入力を生成する。なお、 $L$ 以上 $U$ 以下の整数値を一様ランダムに生成する関数を $\mathrm{rand}(L, U)$ で表す。

1. $l=1,2,\cdots,15$ について、以下の手順で基準点 $(u_l, v_l)$ を生成する。
   1. $u_l=\mathrm{rand}(100, 900),\ v_l=\mathrm{rand}(100, 900)$ とする。ただし、既に生成した基準点のうち、 $(u_l, v_l)$ とのユークリッド距離が100以下であるものが存在した場合は再度 $u_l,\ v_l$ の生成をやり直す。
2. $i=1,2,\cdots,N$について、以下の手順で $i$ 番目の惑星の座標 $(x_i, y_i)$ を生成する。
   1. $m_i=\mathrm{rand}(1, 15)$ を生成する。
   2. $\Delta x_i=\mathrm{rand}(-100, 100),\ \Delta y_i=\mathrm{rand}(-100, 100)$ として、 $x_i=u_{m_i}+\Delta x_i,\ y_i=v_{m_i}+\Delta y_i$ とする。ただし、 $(x_i, y_i)$ が既に生成された惑星の座標と一致する場合は、再度 $m_i,\ \Delta x_i,\ \Delta y_i$ の生成をやり直す。

## ツール（入力ジェネレータ・ビジュアライザ）

- [Web版](https://steiner-space-travel.terry-u16.net/)
  - 以下の機能を持つ。
    - テストケース生成機能
    - 得点計算・ビジュアライズ機能
    - Twitterへの画像共有機能
  - コンテスト期間中、seed=0の結果に限りTwitterでの画像共有が許可されている。[共有された画像一覧](https://twitter.com/search?q=%23SteinerSpaceTravel%20%23visualizer)
    - 共有された画像には若干のネタバレが含まれる可能性がある。ネタバレを完全に防ぎたい場合は、`#SteinerSpaceTravel`で各自ミュートされたい。
- [ローカル版](https://heuristicstorage.blob.core.windows.net/tools-host/steiner-space-travel.zip)
  - 以下の機能を持つ。
    - テストケース生成機能
    - 得点計算・ビジュアライズ機能
    - **複数ケース並列実行・集計機能**
  - 使用するには[.NET Runtime 6](https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0)が必要である。同梱の`README.md`を参照のこと。

## サンプル

### サンプル1

```sample-input
2 1
0 0
200 200
```

```sample-output
200 0
4
1 1
1 2
2 1
1 1
```

このケースは $N=100,\ M=8$ の制約を満たさないため、実際のテストケースとして与えられることはない。

このケースを図示すると下図のようになる。青い大きな円は惑星1を、黄色い円はその他の惑星を、灰色の正方形は宇宙ステーションを表す。

初期状態で惑星1が $(0, 0)$ に、惑星2が $(200, 200)$ に存在する。

宇宙ステーション1を $(200, 0)$ に配置して、惑星1→惑星2→宇宙ステーション1→惑星1という順番で訪問する。

それぞれの移動に必要なエネルギーは以下の通り。

- 惑星1→惑星2: $5^2\times(200^2+200^2)=2\times10^6$
- 惑星2→宇宙ステーション1: $5\times(0^2+200^2)=2\times10^5$
- 宇宙ステーション1→惑星1: $5\times(200^2+0^2)=2\times10^5$

この時のスコアは、 $\mathrm{round}(10^9/(10^3+\sqrt{2\times10^6+2\times10^5+2\times10^5}))=329981$ 点となる。

### サンプル2

```sample-input
3 4
100 100
0 0
0 100
```

```sample-output
150 150
100 100
150 150
100 200
8
1 1
2 4
2 4
1 3
1 2
1 3
2 2
1 1
```

このケースは $N=100,\ M=8$ の制約を満たさないため、実際のテストケースとして与えられることはない。

このケースを図示すると下図のようになる。

以下の条件はいずれも不正な出力となる条件に該当しない。

- 惑星と同じ座標に宇宙ステーションを配置する
- 複数の宇宙ステーションを同じ座標に配置する
- 経由地 $k$ と経由地 $k+1$ が同一である
- 同じ経由地を複数回訪問する
- 訪問しない宇宙ステーションが存在する

よってこの出力は有効であり、 $544467$ 点が得られる。
