#![allow(non_snake_case, unused_imports, unused_macros)]

use proconio::{input, marker::*, source::Source};
use rand::prelude::*;
use std::{
    collections::{BTreeMap, BTreeSet, HashSet},
    io::prelude::*,
};
use svg::node::element::{path::Data, Circle, Path, Rectangle};

pub trait SetMinMax {
    fn setmin(&mut self, v: Self) -> bool;
    fn setmax(&mut self, v: Self) -> bool;
}
impl<T> SetMinMax for T
where
    T: PartialOrd,
{
    fn setmin(&mut self, v: T) -> bool {
        *self > v && {
            *self = v;
            true
        }
    }
    fn setmax(&mut self, v: T) -> bool {
        *self < v && {
            *self = v;
            true
        }
    }
}

#[macro_export]
macro_rules! mat {
	($($e:expr),*) => { Vec::from(vec![$($e),*]) };
	($($e:expr,)*) => { Vec::from(vec![$($e),*]) };
	($e:expr; $d:expr) => { Vec::from(vec![$e; $d]) };
	($e:expr; $d:expr $(; $ds:expr)+) => { Vec::from(vec![mat![$e $(; $ds)*]; $d]) };
}

#[derive(Clone)]
pub struct Output {
    r: Vec<usize>,
    path: Vec<(i32, i32)>,
}

const N: usize = 50;
const M: usize = 8;

pub struct Input {
    n: usize,
    m: usize,
    planets: Vec<(i32, i32)>,
}

impl std::fmt::Display for Input {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        writeln!(f, "{} {}", self.n, self.m)?;
        for i in 0..N {
            writeln!(f, "{} {}", self.planets[i].0, self.planets[i].1)?;
        }
        Ok(())
    }
}

pub fn parse_input(f: &str) -> Input {
    todo!();
    /*
    let f = proconio::source::once::OnceSource::from(f);
    input! {
        from f,
        a: [(i32, i32, i32, i32); N]
    }
    let from = a.iter().map(|&(x, y, _, _)| (x, y)).collect();
    let to = a.iter().map(|&(_, _, x, y)| (x, y)).collect();
    */
}

pub fn parse_output(_input: &Input, f: &str) -> Vec<Output> {
    todo!();

    /*
    let mut out = vec![];
    let mut f = proconio::source::once::OnceSource::from(f);
    while !f.is_empty() {
        input! {
            from &mut f,
            r: [usize],
            path: [(i32, i32)],
        }
        out.push(Output {
            r: r.into_iter().map(|i| i - 1).collect(),
            path,
        });
    }
    out
    */
}

fn dist((x1, y1): (i32, i32), (x2, y2): (i32, i32)) -> i32 {
    (x1 - x2).abs() + (y1 - y2).abs()
}

pub fn gen(seed: u64) -> Input {
    let mut rng = rand_chacha::ChaCha20Rng::seed_from_u64(seed);
    let mut pivots = vec![];

    while pivots.len() < 15 {
        let u = rng.gen_range(100..=900);
        let v = rng.gen_range(100..=900);

		let near = pivots.iter().any(|(x, y)| {
			let dx = u - x;
			let dy = v - y;
			dx * dx + dy * dy <= 100 * 100
		});

		if near {
			continue;
		}

        pivots.push((u, v));
    }

    let mut planets = vec![];
    let mut set = HashSet::new();
    while planets.len() < N {
        let k = rng.gen_range(0..pivots.len());
        let (u, v) = pivots[k];
        let dx = rng.gen_range(-100..=100);
        let dy = rng.gen_range(-100..=100);
        let x = u + dx;
        let y = v + dy;

        if !set.insert((x, y)) {
            continue;
        }

        planets.push((x, y));
    }

    Input {
        n: N,
        m: M,
        planets,
    }
}

fn rect(x: i32, y: i32, w: i32, h: i32, fill: &str) -> Rectangle {
    Rectangle::new()
        .set("x", x)
        .set("y", y)
        .set("width", w)
        .set("height", h)
        .set("fill", fill)
}

pub fn get_max_t(out: &Output) -> i32 {
    let mut time = 0;
    for i in 1..out.path.len() {
        time += dist(out.path[i - 1], out.path[i]);
    }
    time
}
