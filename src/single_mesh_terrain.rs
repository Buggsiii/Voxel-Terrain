use bevy::{
    prelude::*,
    render::{mesh::Indices, render_asset::RenderAssetUsages, render_resource::PrimitiveTopology},
};
use noise::{NoiseFn, Perlin};

const SIZE: i8 = 16;
const SCALE: f64 = 0.1;

pub fn setup(
    mut commands: Commands,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<StandardMaterial>>,
) {
    for x in -4..4 {
        for z in -4..4 {
            let cube_terrain_handle: Handle<Mesh> =
                meshes.add(create_terrain_mesh(x * SIZE, z * SIZE));
            commands.spawn(PbrBundle {
                mesh: cube_terrain_handle,
                material: materials.add(Color::srgba(0.3, 0.5, 0.3, 1.0)),
                transform: Transform::from_translation(Vec3::new(
                    (x * SIZE) as f32,
                    -SIZE as f32,
                    (z * SIZE) as f32,
                )),
                ..default()
            });
        }
    }
}

fn create_terrain_mesh(x_pos: i8, z_pos: i8) -> Mesh {
    let perlin = Perlin::new(0);

    let mut vertices = Vec::new();
    let mut normals = Vec::new();
    let mut indices = Vec::new();

    for x in 0..SIZE {
        for y in 0..SIZE {
            for z in 0..SIZE {
                let val = perlin.get([
                    (x_pos + x) as f64 * SCALE,
                    y as f64 * SCALE,
                    (z_pos + z) as f64 * SCALE,
                ]) as f32;

                if val < -0.2 {
                    continue;
                }

                // Vertices
                // top
                vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32 + 1.0));

                // bottom
                vertices.push(Vec3::new(x as f32, y as f32, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32, y as f32, z as f32 + 1.0));

                // right
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 0.0, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32));

                // left
                vertices.push(Vec3::new(x as f32, y as f32, z as f32));
                vertices.push(Vec3::new(x as f32, y as f32, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32));

                // back
                vertices.push(Vec3::new(x as f32, y as f32, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32 + 1.0));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32 + 1.0));

                // forward
                vertices.push(Vec3::new(x as f32, y as f32, z as f32));
                vertices.push(Vec3::new(x as f32, y as f32 + 1.0, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32 + 1.0, z as f32));
                vertices.push(Vec3::new(x as f32 + 1.0, y as f32, z as f32));

                // Normals
                // top
                normals.push(Vec3::Y);
                normals.push(Vec3::Y);
                normals.push(Vec3::Y);
                normals.push(Vec3::Y);

                // bottom
                normals.push(-Vec3::Y);
                normals.push(-Vec3::Y);
                normals.push(-Vec3::Y);
                normals.push(-Vec3::Y);

                // right
                normals.push(Vec3::X);
                normals.push(Vec3::X);
                normals.push(Vec3::X);
                normals.push(Vec3::X);

                // left
                normals.push(-Vec3::X);
                normals.push(-Vec3::X);
                normals.push(-Vec3::X);
                normals.push(-Vec3::X);

                // back
                normals.push(Vec3::Z);
                normals.push(Vec3::Z);
                normals.push(Vec3::Z);
                normals.push(Vec3::Z);

                // forward
                normals.push(-Vec3::Z);
                normals.push(-Vec3::Z);
                normals.push(-Vec3::Z);
                normals.push(-Vec3::Z);

                // Indices
                let i = vertices.len() as u32;

                // top
                indices.push(i - 24);
                indices.push(i - 21);
                indices.push(i - 23);
                indices.push(i - 23);
                indices.push(i - 21);
                indices.push(i - 22);

                // bottom
                indices.push(i - 20);
                indices.push(i - 19);
                indices.push(i - 17);
                indices.push(i - 19);
                indices.push(i - 18);
                indices.push(i - 17);

                // right
                indices.push(i - 16);
                indices.push(i - 13);
                indices.push(i - 15);
                indices.push(i - 15);
                indices.push(i - 13);
                indices.push(i - 14);

                // left
                indices.push(i - 12);
                indices.push(i - 11);
                indices.push(i - 9);
                indices.push(i - 11);
                indices.push(i - 10);
                indices.push(i - 9);

                // back
                indices.push(i - 8);
                indices.push(i - 5);
                indices.push(i - 7);
                indices.push(i - 7);
                indices.push(i - 5);
                indices.push(i - 6);

                // forward
                indices.push(i - 4);
                indices.push(i - 3);
                indices.push(i - 1);
                indices.push(i - 3);
                indices.push(i - 2);
                indices.push(i - 1);
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
