use bevy::{
    color::palettes::css::WHITE,
    pbr::wireframe::{WireframeConfig, WireframePlugin},
    prelude::*,
    window::{PresentMode, Window, WindowPlugin},
};

mod gui_plugin;
use gui_plugin::GuiPlugin;
mod single_mesh_terrain;
mod unoptimized_terrain;

const SPEED: f32 = 10.0;
const ROT_SPEED: f32 = 1.0;

fn main() {
    App::new()
        .add_plugins((
            DefaultPlugins.set(WindowPlugin {
                primary_window: Some(Window {
                    present_mode: PresentMode::Immediate,
                    ..default()
                }),
                ..default()
            }),
            WireframePlugin,
            GuiPlugin,
        ))
        .insert_resource(WireframeConfig {
            global: false,
            default_color: WHITE.into(),
        })
        .add_systems(Startup, (setup, setup_camera))
        // Unoptimized terrain
        // .add_systems(Startup, unoptimized_terrain::setup)
        // Single mesh terrain
        .add_systems(Startup, single_mesh_terrain::setup)
        .add_systems(Update, input_handler)
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
            // transform: Transform::from_xyz(-15.0, 5.0, -15.0).looking_at(Vec3::ZERO, Vec3::Y),
            transform: Transform::from_xyz(0.0, 1.8, 0.0),
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
    let delta_time = time.delta_seconds();

    for mut transform in &mut query {
        if keyboard_input.pressed(KeyCode::KeyW) {
            let dir = transform.local_z();
            transform.translation -= dir * SPEED * delta_time;
        }
        if keyboard_input.pressed(KeyCode::KeyS) {
            let dir = transform.local_z();
            transform.translation += dir * SPEED * delta_time;
        }

        if keyboard_input.pressed(KeyCode::KeyA) {
            let dir = transform.local_x();
            transform.translation -= dir * SPEED * delta_time;
        }
        if keyboard_input.pressed(KeyCode::KeyD) {
            let dir = transform.local_x();
            transform.translation += dir * SPEED * delta_time;
        }

        if keyboard_input.pressed(KeyCode::ShiftLeft) {
            transform.translation.y += SPEED * delta_time;
        }
        if keyboard_input.pressed(KeyCode::ControlLeft) {
            transform.translation.y -= SPEED * delta_time;
        }

        if keyboard_input.pressed(KeyCode::ArrowUp) {
            transform.rotate_local_x(ROT_SPEED * delta_time);
        }
        if keyboard_input.pressed(KeyCode::ArrowDown) {
            transform.rotate_local_x(-ROT_SPEED * delta_time);
        }
        if keyboard_input.pressed(KeyCode::ArrowLeft) {
            transform.rotate_y(ROT_SPEED * delta_time);
        }
        if keyboard_input.pressed(KeyCode::ArrowRight) {
            transform.rotate_y(-ROT_SPEED * delta_time);
        }
    }
}
