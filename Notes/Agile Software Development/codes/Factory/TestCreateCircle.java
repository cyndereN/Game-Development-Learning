public void testCreateCircle() throws Exception
{
    Shape s = factory.make("circle");
    assert(s instanceof Circle);
}