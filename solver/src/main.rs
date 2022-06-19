use itertools::Itertools;
use proconio::*;
use rand::prelude::*;

const MAP_SIZE: i32 = 1000;
const MULTIPLIER: i64 = 5;

#[allow(unused_macros)]
macro_rules! chmin {
    ($base:expr, $($cmps:expr),+ $(,)*) => {{
        let cmp_min = min!($($cmps),+);
        if $base > cmp_min {
            $base = cmp_min;
            true
        } else {
            false
        }
    }};
}

#[allow(unused_macros)]
macro_rules! chmax {
    ($base:expr, $($cmps:expr),+ $(,)*) => {{
        let cmp_max = max!($($cmps),+);
        if $base < cmp_max {
            $base = cmp_max;
            true
        } else {
            false
        }
    }};
}

#[allow(unused_macros)]
macro_rules! min {
    ($a:expr $(,)*) => {{
        $a
    }};
    ($a:expr, $b:expr $(,)*) => {{
        std::cmp::min($a, $b)
    }};
    ($a:expr, $($rest:expr),+ $(,)*) => {{
        std::cmp::min($a, min!($($rest),+))
    }};
}

#[allow(unused_macros)]
macro_rules! max {
    ($a:expr $(,)*) => {{
        $a
    }};
    ($a:expr, $b:expr $(,)*) => {{
        std::cmp::max($a, $b)
    }};
    ($a:expr, $($rest:expr),+ $(,)*) => {{
        std::cmp::max($a, max!($($rest),+))
    }};
}

#[allow(unused_macros)]
macro_rules! mat {
    ($e:expr; $d:expr) => { vec![$e; $d] };
    ($e:expr; $d:expr $(; $ds:expr)+) => { vec![mat![$e $(; $ds)*]; $d] };
}

#[derive(Debug, Clone)]
struct Input {
    n: usize,
    m: usize,
    points: Vec<Point>,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, PartialOrd, Ord)]
struct Point {
    x: i32,
    y: i32,
}

impl Point {
    fn new(x: i32, y: i32) -> Self {
        Self { x, y }
    }

    fn dist_sq(&self, other: &Self) -> i64 {
        let dx = self.x - other.x;
        let dy = self.y - other.y;
        (dx * dx + dy * dy) as i64
    }
}

#[derive(Debug, Clone)]
struct State {
    points: Vec<Point>,
    orders: Vec<usize>,
}

impl State {
    fn init(input: &Input) -> Self {
        let mut points = input.points.clone();

        for _ in 0..input.m {
            points.push(points[0]);
        }

        let mut orders = vec![];
        for i in 0..input.n {
            orders.push(i);
        }
        orders.push(0);

        Self::new(points, orders)
    }

    fn new(points: Vec<Point>, orders: Vec<usize>) -> Self {
        Self { points, orders }
    }

    fn calc_score_all(&self, input: &Input) -> i64 {
        let mut score = 0;

        for (&prev, &next) in self.orders.iter().tuple_windows() {
            let mul0 = get_score_mul(prev, input.n);
            let mul1 = get_score_mul(next, input.n);
            let dist_sq = self.points[prev].dist_sq(&self.points[next]);
            score += dist_sq * mul0 * mul1;
        }

        score
    }
}

fn main() {
    let input = read_input();
    let state = solve(&input);
    write_output(&input, &state);

    let score = state.calc_score_all(&input);
    eprintln!("score: {}", score);
}

fn read_input() -> Input {
    input! {
        n: usize,
        m: usize,
    }

    let mut points = vec![];

    for _ in 0..n {
        input! {
            x: i32,
            y: i32
        }
        points.push(Point::new(x, y));
    }

    Input { n, m, points }
}

fn solve(input: &Input) -> State {
    let solution = State::init(&input);
    let solution = annealing(&input, solution, 1.0);
    solution
}

fn annealing(input: &Input, initial_solution: State, duration: f64) -> State {
    let mut solution = initial_solution;
    let mut best_solution = solution.clone();
    let mut current_score = solution.calc_score_all(input);
    let initial_score = current_score;
    let mut best_score = current_score;

    let mut all_iter = 0;
    let mut valid_iter = 0;
    let mut accepted_count = 0;
    let mut update_count = 0;
    let mut rng = rand_pcg::Pcg64Mcg::new(42);

    let duration_inv = 1.0 / duration;
    let since = std::time::Instant::now();
    let mut time = 0.0;

    let temp0 = 1e5;
    let temp1 = 1e2;
    let mut inv_temp = 1.0 / temp0;

    while time < 1.0 {
        all_iter += 1;
        if (all_iter & ((1 << 6) - 1)) == 0 {
            time = (std::time::Instant::now() - since).as_secs_f64() * duration_inv;
            let temp = f64::powf(temp0, 1.0 - time) * f64::powf(temp1, time);
            inv_temp = 1.0 / temp;
        }

        // 変形
        let mod_station = rng.gen_range(0..100) < 10;

        if mod_station {
            let station_id = input.n + rng.gen_range(0..input.m);
            let mut temp_orders = solution.orders.clone();
            temp_orders.retain(|&v| v != station_id);

            let mut p = solution.points[station_id];
            const DELTA: i32 = 100;
            p.x = rng.gen_range((p.x - DELTA).max(0)..=(p.x + DELTA).min(MAP_SIZE));
            p.y = rng.gen_range((p.y - DELTA).max(0)..=(p.y + DELTA).min(MAP_SIZE));
            let mut new_orders = vec![];

            for (&prev, &next) in temp_orders.iter().tuple_windows() {
                new_orders.push(prev);
                let mul0 = get_score_mul(prev, input.n);
                let mul1 = get_score_mul(next, input.n);
                let p0 = solution.points[prev];
                let p1 = solution.points[next];
                let old_dist = p0.dist_sq(&p1) * mul0 * mul1;
                let new_dist = p0.dist_sq(&p) * mul0 + p.dist_sq(&p1) * mul1;
                if new_dist < old_dist {
                    new_orders.push(station_id);
                }
            }

            new_orders.push(0);

            let mut new_solution = State::new(solution.points.clone(), new_orders);
            new_solution.points[station_id] = p;
            let new_score = new_solution.calc_score_all(input);
            let score_diff = new_score - current_score;

            if score_diff <= 0 || rng.gen_bool(f64::exp(-score_diff as f64 * inv_temp)) {
                // 解の更新
                current_score = new_score;
                accepted_count += 1;
                solution = new_solution;

                if chmin!(best_score, current_score) {
                    best_solution = solution.clone();
                    update_count += 1;
                }
            }
        } else {
            let from = rng.gen_range(1..(solution.orders.len() - 1));
            let to = rng.gen_range((from + 1)..solution.orders.len());

            let i0 = solution.orders[from - 1];
            let i1 = solution.orders[from];
            let i2 = solution.orders[to - 1];
            let i3 = solution.orders[to];
            let p0 = solution.points[i0];
            let p1 = solution.points[i1];
            let p2 = solution.points[i2];
            let p3 = solution.points[i3];
            let mul0 = get_score_mul(i0, input.n);
            let mul1 = get_score_mul(i1, input.n);
            let mul2 = get_score_mul(i2, input.n);
            let mul3 = get_score_mul(i3, input.n);

            let d01 = p0.dist_sq(&p1) * mul0 * mul1;
            let d23 = p2.dist_sq(&p3) * mul2 * mul3;
            let d02 = p0.dist_sq(&p2) * mul0 * mul2;
            let d13 = p1.dist_sq(&p3) * mul1 * mul3;

            // スコア計算
            let score_diff = d02 + d13 - d01 - d23;
            let new_score = current_score + score_diff;

            if score_diff <= 0 || rng.gen_bool(f64::exp(-score_diff as f64 * inv_temp)) {
                // 解の更新
                current_score = new_score;
                accepted_count += 1;
                solution.orders[from..to].reverse();

                if chmin!(best_score, current_score) {
                    best_solution = solution.clone();
                    update_count += 1;
                }
            }
        }

        valid_iter += 1;
    }

    eprintln!("===== annealing =====");
    eprintln!("initial_score : {}", initial_score);
    eprintln!("score         : {}", best_score);
    eprintln!("all iter      : {}", all_iter);
    eprintln!("valid iter    : {}", valid_iter);
    eprintln!("accepted      : {}", accepted_count);
    eprintln!("updated       : {}", update_count);
    eprintln!("");

    best_solution
}

fn get_score_mul(v: usize, threshold: usize) -> i64 {
    if v < threshold {
        MULTIPLIER
    } else {
        1
    }
}

fn write_output(input: &Input, solution: &State) {
    for i in 0..input.m {
        let p = solution.points[i + input.n];
        println!("{} {}", p.x, p.y);
    }

    println!("{}", solution.orders.len());

    for v in solution.orders.iter() {
        println!("{}", v);
    }
}
