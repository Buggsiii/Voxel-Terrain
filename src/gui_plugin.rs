use bevy::prelude::*;
use bevy_egui::{EguiContexts, EguiPlugin};
use std::collections::VecDeque;

pub struct GuiPlugin;

impl Plugin for GuiPlugin {
    fn build(&self, app: &mut App) {
        app.add_plugins(EguiPlugin).add_systems(Update, show_window);
    }
}

fn show_window(
    mut contexts: EguiContexts,
    time: Res<Time>,
    mut frame_times: Local<VecDeque<f64>>,
    mut enable_wireframe: Local<bool>,
    mut enable_occlusion_culling: Local<bool>,
) {
    let fps = 1.0 / time.delta_seconds_f64();
    frame_times.push_back(fps);

    // Keep only the last 100 frame times
    if frame_times.len() > 1000 {
        frame_times.pop_front();
    }

    let average_fps: f64 = frame_times.iter().sum::<f64>() / frame_times.len() as f64;

    egui::Window::new("Voxel Terrain").show(contexts.ctx_mut(), |ui| {
        ui.label(format!("FPS: {:.0}", fps));
        ui.label(format!("Average FPS: {:.0}", average_fps));

        ui.separator();

        ui.label("Select terrain type:");
        ui.horizontal(|ui| {
            if ui.button("Unoptimized").clicked() {
                println!("Switching to unoptimized terrain");
                // app.add_systems(Startup, unoptimized_terrain::setup);
            }
            if ui.button("Single Mesh").clicked() {
                println!("Switching to single mesh terrain");
                // app.add_systems(Startup, single_mesh_terrain::setup);
            }
            if ui.button("Culled Mesh").clicked() {
                println!("Switching to single mesh terrain");
                // app.add_systems(Startup, single_mesh_terrain::setup);
            }
        });

        ui.separator();

        ui.label("Settings:");
        ui.checkbox(&mut enable_wireframe, "Wireframe");
        ui.checkbox(&mut enable_occlusion_culling, "Occlusion Culling");
    });
}
