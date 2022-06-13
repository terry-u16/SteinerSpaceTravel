use itertools::Itertools;
use proconio::*;

const CENTER: i32 = 500;
const MULTIPLIER: i64 = 10;

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

    fn dist_sq(&self, other: &Self) -> i32 {
        let dx = self.x - other.x;
        let dy = self.y - other.y;
        dx * dx + dy * dy
    }
}

#[derive(Debug, Clone)]
struct State {
    points: Vec<Point>,
    orders: Vec<usize>,
}

impl State {
    fn init(input: &Input) -> Self {
        let mut points = vec![Point::new(CENTER, CENTER)];
        points.append(&mut input.points.clone());

        for _ in 0..input.m {
            points.push(Point::new(CENTER, CENTER));
        }

        let mut orders = vec![];
        for i in 0..=input.n {
            orders.push(i);
        }
        orders.push(0);

        Self::new(points, orders)
    }

    fn new(points: Vec<Point>, orders: Vec<usize>) -> Self {
        Self { points, orders }
    }

    fn calc_score_all(&self, input: &Input) -> i64 {
        fn calc_mul(v: usize, threshold: usize) -> i64 {
            if v == 0 || v > threshold {
                1
            } else {
                MULTIPLIER
            }
        }
        let mut score = 0;

        for (&prev, &next) in self.orders.iter().tuple_windows() {
            let mul0 = calc_mul(prev, input.n);
            let mul1 = calc_mul(next, input.n);
            let dist_sq = self.points[prev].dist_sq(&self.points[next]) as i64;
            score += dist_sq * mul0 * mul1;
        }

        score
    }
}

fn main() {
    let input = read_input();
    let state = State::init(&input);
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
