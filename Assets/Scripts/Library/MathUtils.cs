public static class MathUtils
{
	public static float Remap(float x, float a, float b, float c, float d)
	{
		return (x - a) / (b - a) * (d - c) + c;
	}
}