use bevy::core_pipeline::core_2d::graph::input;
use bevy::prelude::*;
use bevy::render::{
    mesh::{Indices, VertexAttributeValues},
    render_asset::RenderAssetUsages,
    render_resource::PrimitiveTopology,
};

fn main() {
    App::new()
        .add_plugins(DefaultPlugins)
        .add_systems(Startup, setup)
        .add_systems(Update, input_handler)
        .run();
}

fn setup(
    mut commands: Commands,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<StandardMaterial>>,
) {
    commands.spawn(Camera3dBundle {
        transform: Transform::from_xyz(15.0, 5.0, 15.0).looking_at(Vec3::ZERO, Vec3::Y),
        ..default()
    });

    commands.spawn(DirectionalLightBundle {
        transform: Transform::from_rotation(Quat::from_rotation_x(-std::f32::consts::FRAC_PI_4)),
        ..default()
    });

    commands.spawn(
        PbrBundle {
            mesh: meshes.add(Plane3d::default().mesh().size(20., 20.)),
            material: materials.add(Color::srgb(0.3, 0.5, 0.3)),
            ..default()
        }
    );

    // let cube_mesh_handle: Handle<Mesh> = create_cube_mesh();
    // commands.spawn(PbrBundle {
    //     mesh: cube_mesh_handle,
    //     ..default()
    // });
}

fn input_handler(
    keyboard_input: Res<ButtonInput<KeyCode>>,
    mesh_query: Query<&Handle<Mesh>, With<Camera>>,
    mut meshes: ResMut<Assets<Mesh>>,
    mut query: Query<&mut Transform, With<Camera>>,
    time: Res<Time>
) {
    for mut transform in &mut query {
        if keyboard_input.pressed(KeyCode::KeyW) {
            let dir = transform.local_z();
            transform.translation -= dir * 0.1;
        }
        if keyboard_input.pressed(KeyCode::KeyS) {
            let dir = transform.local_z();
            transform.translation += dir * 0.1;
        }
        if keyboard_input.pressed(KeyCode::KeyA) {
            let dir = transform.local_x();
            transform.translation -= dir * 0.1;
        }
        if keyboard_input.pressed(KeyCode::KeyD) {
            let dir = transform.local_x();
            transform.translation += dir * 0.1;
        }
    }
}

// fn create_cube_mesh() -> Mesh {

// }