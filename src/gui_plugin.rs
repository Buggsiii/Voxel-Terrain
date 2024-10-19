use bevy::prelude::*;
use bevy_egui::{EguiContexts, EguiPlugin};
use std::collections::VecDeque;

pub struct GuiPlugin;

impl Plugin for GuiPlugin {
    fn build(&self, app: &mut App) {
        app.add_plugins(EguiPlugin)
            .init_resource::<UiState>()
            .add_systems(Update, show_window);
    }
}

#[derive(Default, Resource)]
pub struct UiState {
    enable_wireframe: bool,
    enable_occlusion_culling: bool,
    render_distance: u16,
}

fn show_window(
    mut contexts: EguiContexts,
    time: Res<Time>,
    mut frame_times: Local<VecDeque<f64>>,
    mut ui_state: ResMut<UiState>,
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
        ui.checkbox(&mut ui_state.enable_wireframe, "Wireframe");
        ui.checkbox(&mut ui_state.enable_occlusion_culling, "Occlusion Culling");
        ui.add(egui::Slider::new(&mut ui_state.render_distance, 0..=32).text("Render Distance"));
    });
}
