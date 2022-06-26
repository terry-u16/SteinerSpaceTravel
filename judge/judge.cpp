#include <cassert>
#include <cmath>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <optional>
#include <tuple>
#include <vector>

using ll = long long;
using std::vector;
constexpr ll TYPE_PLANET = 1;
constexpr ll TYPE_STATION = 2;
constexpr ll ALPHA = 5;

// 構造体定義
#pragma region Structs

struct Point {
    ll x;
    ll y;
    Point(ll x, ll y) : x(x), y(y) {}

    ll dist_sq(const Point& other) {
        ll dx = x - other.x;
        ll dy = y - other.y;
        return dx * dx + dy * dy;
    }
};

struct Visit {
    ll type;
    ll index;
    Visit(ll type, ll index) : type(type), index(index) {}
};

struct Input {
    ll n;
    ll m;
    vector<Point> planets;

    Input(ll n, ll m, vector<Point>&& planets) : n(n), m(m), planets(planets) {}
};

struct Output {
    vector<Point> stations;
    vector<Visit> visits;
    Output(vector<Point>&& stations, vector<Visit>&& visits) : stations(stations), visits(visits) {}
};

#pragma endregion

// 入出力
#pragma region IO

// 入力の読み込みを行う
Input read_input(std::istream& stream) {
    ll n, m;
    vector<Point> planets;

    stream >> n >> m;

    for (int i = 0; i < n; i++) {
        ll x, y;
        stream >> x >> y;
        planets.emplace_back(x, y);
    }

    return Input(n, m, std::move(planets));
}

// 全惑星を訪問したかチェック
bool check_visited_all(const Input& input, const Output& output) {
    vector<bool> visited(input.n);

    for (const auto& v : output.visits) {
        if (v.type == TYPE_PLANET) {
            visited[v.index] = true;
        }
    }

    for (const auto& v : visited) {
        if (!v) {
            return false;
        }
    }

    return true;
}

// 惑星1かどうかチェック
bool is_earth(const Visit& visit) { return visit.type == TYPE_PLANET && visit.index == 0; }

// 出力の読み込みを行う
Output read_output(std::istream& stream, const Input& input) {
    constexpr ll min_xy = 0;
    constexpr ll max_xy = 1000;
    constexpr ll min_visit = 1;
    constexpr ll max_visit = 100000;
    ll v;
    vector<Point> stations;
    vector<Visit> visits;

    for (int i = 0; i < input.m; i++) {
        ll x, y;
        stream >> x >> y;
        assert(min_xy <= x && x <= max_xy);
        assert(min_xy <= y && y <= max_xy);
        stations.emplace_back(x, y);
    }

    stream >> v;
    assert(min_visit <= v && v <= max_visit);

    for (int i = 0; i < v; i++) {
        ll type, index;
        stream >> type >> index;

        switch (type) {
            case TYPE_PLANET:
                assert(1 <= index && index <= input.n);
                break;
            case TYPE_STATION:
                assert(1 <= index && index <= input.m);
                break;
            default:
                abort();
        }

        // 0-indexedに変換
        visits.emplace_back(type, index - 1);
    }

    Output output(std::move(stations), std::move(visits));
    assert(check_visited_all(input, output));
    assert(is_earth(output.visits[0]));
    assert(is_earth(output.visits[output.visits.size() - 1]));

    return output;
}

#pragma endregion

// ジャッジ
#pragma region Judge

Point get_point(const Input& input, const Output& output, const Visit& visit) {
    switch (visit.type) {
        case TYPE_PLANET:
            return input.planets[visit.index];
        case TYPE_STATION:
            return output.stations[visit.index];
        default:
            abort();
    }
}

// 経路1つ分の消費エネルギーを計算する
ll calc_energy_single(const Input& input, const Output& output, const Visit& prev, const Visit& next) {
    Point p0 = get_point(input, output, prev);
    Point p1 = get_point(input, output, next);
    ll energy = p0.dist_sq(p1);
    if (prev.type == TYPE_PLANET) {
        energy *= ALPHA;
    }
    if (next.type == TYPE_PLANET) {
        energy *= ALPHA;
    }
    return energy;
}

// 消費エネルギーを計算する
ll calc_energy(const Input& input, const Output& output) {
    ll energy = 0;

    for (int i = 0; i + 1 < output.visits.size(); i++) {
        Visit prev = output.visits[i];
        Visit next = output.visits[i + 1];
        energy += calc_energy_single(input, output, prev, next);
    }

    return energy;
}

// スコアを計算する
ll calc_score(const Input& input, const Output& output) {
    ll energy = calc_energy(input, output);
    double score_f = 1e9 / (1e3 + std::sqrt(energy));
    return static_cast<ll>(std::round(score_f));
}

#pragma endregion

int main(int argc, char** argv) {
    // テストケースの入力ファイルの input stream
    std::ifstream input_ifs(argv[1]);
    // テストケースの出力ファイルの input stream
    std::ifstream output_ifs(argv[2]);
    // 提出されたコードのファイルの input stream
    std::ifstream code_ifs(argv[3]);
    // スコアファイル（スコア問題のみ利用）の output stream
    std::ofstream score_ofs(argv[4]);

    // 入力ファイルの読み込み
    Input input = read_input(input_ifs);

    // 出力ファイルの読み込み
    Output output = read_output(std::cin, input);

    // スコア計算
    ll score = calc_score(input, output);
    score_ofs << score << std::endl;

    return 0;
}
