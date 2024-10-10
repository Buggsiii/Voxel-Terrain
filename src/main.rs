use bevy::{
    color::palettes::css::WHITE,
    diagnostic::{FrameTimeDiagnosticsPlugin, LogDiagnosticsPlugin},
    pbr::wireframe::{WireframeConfig, WireframePlugin},
    prelude::*,
};
use bevy_egui::{EguiContexts, EguiPlugin};

mod single_mesh_terrain;
mod unoptimized_terrain;

fn main() {
    App::new()
        .add_plugins((
            DefaultPlugins,
            FrameTimeDiagnosticsPlugin,
            LogDiagnosticsPlugin::default(),
            WireframePlugin,
            EguiPlugin,
        ))
        .insert_resource(WireframeConfig {
            global: true,
            default_color: WHITE.into(),
        })
        .add_systems(Startup, (setup, setup_camera))
        // Unoptimized terrain
        // .add_systems(Startup, unoptimized_terrain::setup)
        // Single mesh terrain
        .add_systems(Startup, single_mesh_terrain::setup)
        .add_systems(Update, (input_handler, gui))
        .run();
}

fn setup(mut commands: Commands) {
    commands.spawn(DirectionalLightBundle {
        transform: Transform::from_rotation(Quat::from_rotation_x(-std::f32::consts::FRAC_PI_4)),
        directional_light: DirectionalLight {
            shadows_enabled: true,
            ..default()
        },
        ..default()
    });
}

fn setup_camera(mut commands: Commands) {
    commands.spawn((
        Camera3dBundle {
            transform: Transform::from_xyz(-15.0, 5.0, -15.0).looking_at(Vec3::ZERO, Vec3::Y),
            ..default()
        },
        // FogSettings {
        //     color: Color::srgb(0.168627451, 0.17254902, 0.184313725),
        //     falloff: FogFalloff::Linear {
        //         start: 5.0,
        //         end: 20.0,
        //     },
        //     ..default()
        // },
    ));
}

fn input_handler(
    keyboard_input: Res<ButtonInput<KeyCode>>,
    mut query: Query<&mut Transform, With<Camera>>,
    time: Res<Time>,
) {
    let speed = 10.0;
    let rot_speed = 1.;
    let delta_time = time.delta_seconds();

    for mut transform in &mut query {
        if keyboard_input.pressed(KeyCode::KeyW) {
            let dir = transform.local_z();
            transform.translation -= dir * speed * delta_time;
        }
        if keyboard_input.pressed(KeyCode::KeyS) {
            let dir = transform.local_z();
            transform.translation += dir * speed * delta_time;
        }

        if keyboard_input.pressed(KeyCode::KeyA) {
            let dir = transform.local_x();
            transform.translation -= dir * speed * delta_time;
        }
        if keyboard_input.pressed(KeyCode::KeyD) {
            let dir = transform.local_x();
            transform.translation += dir * speed * delta_time;
        }

        if keyboard_input.pressed(KeyCode::ShiftLeft) {
            transform.translation.y += speed * delta_time;
        }
        if keyboard_input.pressed(KeyCode::ControlLeft) {
            transform.translation.y -= speed * delta_time;
        }

        if keyboard_input.pressed(KeyCode::ArrowUp) {
            transform.rotate_local_x(rot_speed * delta_time);
        }
        if keyboard_input.pressed(KeyCode::ArrowDown) {
            transform.rotate_local_x(-rot_speed * delta_time);
        }
        if keyboard_input.pressed(KeyCode::ArrowLeft) {
            transform.rotate_y(rot_speed * delta_time);
        }
        if keyboard_input.pressed(KeyCode::ArrowRight) {
            transform.rotate_y(-rot_speed * delta_time);
        }
    }
}

fn gui(mut contexts: EguiContexts) {
    egui::Window::new("Hello").show(contexts.ctx_mut(), |ui| {
        ui.label("Hello World!");
    });
}
