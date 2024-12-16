namespace CRUDS_Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange Declartion & Collecting inputs
            MY_Math mY_Math = new MY_Math();
            int a = 5 , b = 10;
            int expected = 15;
            // Act   Call Method 
            int actual =  mY_Math.Add(a, b);
            // Assert  Compare Actual with expected 
            Assert.Equal(expected, actual);

        }
    }
}