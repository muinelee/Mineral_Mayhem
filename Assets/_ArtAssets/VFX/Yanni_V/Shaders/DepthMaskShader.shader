Shader "Unlit/DepthMaskShader" {

	Subshader{
		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.

		Tags {"Queue" = "Geometry-1"} 

		// Don't draw in the RGBA channels; just the depth buffer
		Cull off
		ColorMask 0

		// Do nothing specific in the pass

		Pass {}
	}
}
