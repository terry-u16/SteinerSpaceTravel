use std::time::Instant;

use crate::rand::Xoshiro256;

const MAP_SIZE: i32 = 1000;
const MULTIPLIER: i64 = 5;

macro_rules! get {
      ($t:ty) => {
          {
              let mut line: String = String::new();
              std::io::stdin().read_line(&mut line).unwrap();
              line.trim().parse::<$t>().unwrap()
          }
      };
      ($($t:ty),*) => {
          {
              let mut line: String = String::new();
              std::io::stdin().read_line(&mut line).unwrap();
              let mut iter = line.split_whitespace();
              (
                  $(iter.next().unwrap().parse::<$t>().unwrap(),)*
              )
          }
      };
      ($t:ty; $n:expr) => {
          (0..$n).map(|_|
              get!($t)
          ).collect::<Vec<_>>()
      };
      ($($t:ty),*; $n:expr) => {
          (0..$n).map(|_|
              get!($($t),*)
          ).collect::<Vec<_>>()
      };
      ($t:ty ;;) => {
          {
              let mut line: String = String::new();
              std::io::stdin().read_line(&mut line).unwrap();
              line.split_whitespace()
                  .map(|t| t.parse::<$t>().unwrap())
                  .collect::<Vec<_>>()
          }
      };
      ($t:ty ;; $n:expr) => {
          (0..$n).map(|_| get!($t ;;)).collect::<Vec<_>>()
      };
}

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
    since: Instant,
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

        for w in self.orders.windows(2) {
            let (prev, next) = (w[0], w[1]);
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

    let elapsed = (Instant::now() - input.since).as_millis();
    eprintln!("elapsed: {}ms", elapsed);
}

fn read_input() -> Input {
    let (n, m) = get!(usize, usize);
    let since = Instant::now();

    let mut points = vec![];

    for _ in 0..n {
        let (x, y) = get!(i32, i32);
        points.push(Point::new(x, y));
    }

    Input {
        n,
        m,
        points,
        since,
    }
}

fn solve(input: &Input) -> State {
    let solution = State::init(&input);
    let solution = annealing(&input, solution, 0.98);
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
    let mut rng = Xoshiro256::new(42);

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
        let mod_station = rng.gen_usize(0, 100) < 10;

        if mod_station {
            let station_id = input.n + rng.gen_usize(0, input.m);
            let mut temp_orders = solution.orders.clone();
            temp_orders.retain(|&v| v != station_id);

            let mut p = solution.points[station_id];
            const DELTA: i32 = 100;
            p.x = rng.gen_i32((p.x - DELTA).max(0), (p.x + DELTA).min(MAP_SIZE) + 1);
            p.y = rng.gen_i32((p.y - DELTA).max(0), (p.y + DELTA).min(MAP_SIZE) + 1);
            let mut new_orders = vec![];

            for w in temp_orders.windows(2) {
                let (prev, next) = (w[0], w[1]);
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
            let from = rng.gen_usize(1, solution.orders.len() - 1);
            let to = rng.gen_usize(from + 1, solution.orders.len());

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

    for &v in solution.orders.iter() {
        if v < input.n {
            println!("1 {}", v + 1);
        } else {
            println!("2 {}", v + 1 - input.n);
        }
    }
}

mod rand {
    pub(crate) struct Xoshiro256 {
        s0: u64,
        s1: u64,
        s2: u64,
        s3: u64,
    }

    impl Xoshiro256 {
        pub(crate) fn new(mut seed: u64) -> Self {
            let s0 = split_mix_64(&mut seed);
            let s1 = split_mix_64(&mut seed);
            let s2 = split_mix_64(&mut seed);
            let s3 = split_mix_64(&mut seed);
            Self { s0, s1, s2, s3 }
        }

        fn next(&mut self) -> u64 {
            let result = (self.s1 * 5).rotate_left(7) * 9;
            let t = self.s1 << 17;

            self.s2 ^= self.s0;
            self.s3 ^= self.s1;
            self.s1 ^= self.s2;
            self.s0 ^= self.s3;
            self.s2 ^= t;
            self.s3 = self.s3.rotate_left(45);

            result
        }

        pub(crate) fn gen_usize(&mut self, lower: usize, upper: usize) -> usize {
            assert!(lower < upper);
            let count = upper - lower;
            (self.next() % count as u64) as usize + lower
        }

        pub(crate) fn gen_i32(&mut self, lower: i32, upper: i32) -> i32 {
            assert!(lower < upper);
            let count = upper - lower;
            (self.next() % count as u64) as i32 + lower
        }

        pub(crate) fn gen_f64(&mut self) -> f64 {
            const UPPER_MASK: u64 = 0x3ff0000000000000;
            const LOWER_MASK: u64 = 0xfffffffffffff;
            let result = UPPER_MASK | (self.next() & LOWER_MASK);
            let result: f64 = unsafe { std::mem::transmute(result) };
            result - 1.0
        }

        pub(crate) fn gen_bool(&mut self, prob: f64) -> bool {
            self.gen_f64() < prob
        }
    }

    fn split_mix_64(x: &mut u64) -> u64 {
        *x += 0x9e3779b97f4a7c15;
        let mut z = *x;
        z = (z ^ z >> 30) * 0xbf58476d1ce4e5b9;
        z = (z ^ z >> 27) * 0x94d049bb133111eb;
        return z ^ z >> 31;
    }
}
