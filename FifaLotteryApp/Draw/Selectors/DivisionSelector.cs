namespace FifaLotteryApp.Draw
{
    public class DivisionSelector
    {
        private const int NumOfDevisions = 4;

        public int Draw()
        {
            System.Random r = new System.Random();
            int divisionNum = r.Next(1, NumOfDevisions + 1);

            return divisionNum;
        }
    }
}