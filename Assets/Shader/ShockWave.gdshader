shader_type canvas_item;

uniform vec2 center;
uniform float force;
uniform float size;
uniform float thickness;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	//var transformed_position = get_global_transform_with_canvas().origin / get_viewport_rect().size;
	vec2 normUV = normalize(UV);
	
	float ratio = SCREEN_PIXEL_SIZE.x / SCREEN_PIXEL_SIZE.y;
	vec2 scaledUV = (UV - vec2(0.5, 0.0)) / vec2(ratio, 1.0) + vec2(0.5, 0.0);
	float mask = (1.0 - smoothstep(size - thickness - 0.1, size - thickness, length(scaledUV - center))) * (smoothstep(size - 0.2, size - 0.1, length(scaledUV - center)));
	
	vec2 disp = normalize(scaledUV - center) * force * mask;
	//COLOR = vec4(SCREEN_UV - disp, 0.0, 1.0);
	COLOR = texture(SCREEN_TEXTURE, SCREEN_UV - disp);
	//COLOR.rbg = vec3(mask);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
