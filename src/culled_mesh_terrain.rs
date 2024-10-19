use bevy::{
    prelude::*,
    render::{mesh::Indices, render_asset::RenderAssetUsages, render_resource::PrimitiveTopology},
};
use noise::{NoiseFn, Perlin};

const SIZE: u16 = 16;
const SCALE: f64 = 0.1;
const RENDER_DISTANCE: u16 = 16;

pub fn setup(
    mut commands: Commands,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<StandardMaterial>>,
) {
    for x in -(RENDER_DISTANCE as i16)..(RENDER_DISTANCE as i16) {
        for z in -(RENDER_DISTANCE as i16)..(RENDER_DISTANCE as i16) {
            let cube_terrain_handle: Handle<Mesh> =
                meshes.add(create_terrain_mesh(x * SIZE as i16, z * SIZE as i16));
            commands.spawn(PbrBundle {
                mesh: cube_terrain_handle,
                material: materials.add(StandardMaterial {
                    base_color: Color::srgba(0.3, 0.5, 0.3, 1.0),
                    // metallic: 0.0,
                    // specular_transmission: 0.0,
                    perceptual_roughness: 1.0,
                    ..default()
                }),
                transform: Transform::from_translation(Vec3::new(
                    (x * SIZE as i16) as f32,
                    -(SIZE as f32),
                    (z * SIZE as i16) as f32,
                )),
                ..default()
            });
        }
    }
}

fn is_solid(x: i16, y: i16, z: i16, perlin: &Perlin) -> bool {
    let val = perlin.get([x as f64 * SCALE, y as f64 * SCALE, z as f64 * SCALE]) as f32;

    val >= -0.2
}

fn create_terrain_mesh(x_pos: i16, z_pos: i16) -> Mesh {
    let perlin = Perlin::new(0);

    let mut vertices = Vec::new();
    let mut normals = Vec::new();
    let mut indices = Vec::new();

    for x in 0..SIZE {
        for y in 0..SIZE {
            for z in 0..SIZE {
                if !is_solid(x_pos + x as i16, y as i16, z_pos + z as i16, &perlin) {
                    continue;
                }

                let is_top = is_solid(x_pos + x as i16, y as i16 + 1, z_pos + z as i16, &perlin)
                    && y < SIZE - 1;
                let is_bottom =
                    is_solid(x_pos + x as i16, y as i16 - 1, z_pos + z as i16, &perlin) && y > 0;
                let is_right = is_solid(x_pos + x as i16 + 1, y as i16, z_pos + z as i16, &perlin);
                let is_left = is_solid(x_pos + x as i16 - 1, y as i16, z_pos + z as i16, &perlin);
                let is_front = is_solid(x_pos + x as i16, y as i16, z_pos + z as i16 + 1, &perlin);
                let is_back = is_solid(x_pos + x as i16, y as i16, z_pos + z as i16 - 1, &perlin);

                let mut i: u32 = vertices.len() as u32;

                if !is_top {
                    vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32 + 1.0));

                    normals.push(Vec3::Y);
                    normals.push(Vec3::Y);
                    normals.push(Vec3::Y);
                    normals.push(Vec3::Y);

                    i += 4;

                    indices.push(i - 1);
                    indices.push(i - 2);
                    indices.push(i - 3);
                    indices.push(i - 1);
                    indices.push(i - 3);
                    indices.push(i - 4);
                }

                if !is_bottom {
                    vertices.push(Vec3::new(x as f32, y as f32, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32, y as f32, z as f32 + 1.0));

                    normals.push(-Vec3::Y);
                    normals.push(-Vec3::Y);
                    normals.push(-Vec3::Y);
                    normals.push(-Vec3::Y);

                    i += 4;

                    indices.push(i - 4);
                    indices.push(i - 3);
                    indices.push(i - 1);
                    indices.push(i - 3);
                    indices.push(i - 2);
                    indices.push(i - 1);
                }

                if !is_right {
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 0.0, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32));

                    normals.push(Vec3::X);
                    normals.push(Vec3::X);
                    normals.push(Vec3::X);
                    normals.push(Vec3::X);

                    i += 4;

                    indices.push(i - 1);
                    indices.push(i - 2);
                    indices.push(i - 3);
                    indices.push(i - 1);
                    indices.push(i - 3);
                    indices.push(i - 4);
                }

                if !is_left {
                    // left
                    vertices.push(Vec3::new(x as f32, y as f32, z as f32));
                    vertices.push(Vec3::new(x as f32, y as f32, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32));

                    normals.push(-Vec3::X);
                    normals.push(-Vec3::X);
                    normals.push(-Vec3::X);
                    normals.push(-Vec3::X);

                    i += 4;

                    indices.push(i - 4);
                    indices.push(i - 3);
                    indices.push(i - 1);
                    indices.push(i - 3);
                    indices.push(i - 2);
                    indices.push(i - 1);
                }

                if !is_front {
                    vertices.push(Vec3::new(x as f32, y as f32, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32 + 1.0));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32 + 1.0));

                    normals.push(Vec3::Z);
                    normals.push(Vec3::Z);
                    normals.push(Vec3::Z);
                    normals.push(Vec3::Z);

                    i += 4;

                    indices.push(i - 1);
                    indices.push(i - 2);
                    indices.push(i - 3);
                    indices.push(i - 1);
                    indices.push(i - 3);
                    indices.push(i - 4);
                }

                if !is_back {
                    vertices.push(Vec3::new(x as f32, y as f32, z as f32));
                    vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32));
                    vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32));

                    normals.push(-Vec3::Z);
                    normals.push(-Vec3::Z);
                    normals.push(-Vec3::Z);
                    normals.push(-Vec3::Z);

                    i += 4;

                    indices.push(i - 4);
                    indices.push(i - 3);
                    indices.push(i - 1);
                    indices.push(i - 3);
                    indices.push(i - 2);
                    indices.push(i - 1);
                }
            }
        }
    }

    Mesh::new(
        PrimitiveTopology::TriangleList,
        RenderAssetUsages::MAIN_WORLD | RenderAssetUsages::RENDER_WORLD,
    )
    .with_inserted_attribute(Mesh::ATTRIBUTE_POSITION, vertices)
    .with_inserted_attribute(Mesh::ATTRIBUTE_NORMAL, normals)
    .with_inserted_indices(Indices::U32(indices))
}
