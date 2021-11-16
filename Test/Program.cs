using System;
using System.IO;
using System.Timers;

namespace Test
{
    class Program
    {
        static Timer timer;
        static Random random = new Random();
        static string currentFileName;
        static string sourcePath;
        static string alphabet = "ABCDEFGHIJKLMNOPRSTUVYZ";
        static string numbers = "0123456789";
        static char[] randomPlate = new char[8];
        static char[] vehicleId = new char[7];
        static string[] cameras = { "ON", "ARKA" };
        static string[] locationCodes = { "080213011101", "090213011102", "100213011103" };
        static string plateLocation = "973-920-1156-960";
        static string fileExtension = ".jpg";
        static int Scenario = 2;

        static void Main(string[] args)
        {
            timer = new Timer {
                Interval = 5000, AutoReset=true
            };
            timer.Elapsed += CreateFileName;
            timer.Start();
           
            Console.ReadKey();
        }
        public static void CreateRandomPlate()
        {
            for (int i = 0; i < randomPlate.Length; i++)
            {
                if (i < 2)
                {
                    randomPlate[i] = numbers[random.Next(numbers.Length)];
                }
                if (i > 1 && i < 5)
                {
                    randomPlate[i] = alphabet[random.Next(alphabet.Length)];
                }
                if (i > 4 && i < 8)
                {
                    randomPlate[i] = numbers[random.Next(numbers.Length)];
                }
            }
        }
        public static void CreateVehicleId()
        {
            for (int i = 0; i < vehicleId.Length; i++)
            {
                vehicleId[i] = numbers[random.Next(numbers.Length)];
            }
        }
        public static string CreateTime()
        {
            DateTime date = DateTime.Now;
            date = date.AddMinutes(3);
            string time = date.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            return time;
        }
        public static string MoveFile(string destinationPath, string time, int locationIndex, int cameraIndex)
        {

            System.Threading.Thread.Sleep(3000);
            string newPlate = new string(randomPlate);
            string newVehicleId = new string(vehicleId);

            var fileName = string.Concat(newPlate + "_" + cameras[cameraIndex] + "_" + newVehicleId + "_" + locationCodes[locationIndex] + "_" + time + "_" + plateLocation + fileExtension);

            string sourceDirectory = Path.Combine(sourcePath, fileName);
            string destinationDirectory = Path.Combine(destinationPath, fileName);

            File.Move(sourcePath + "\\" + currentFileName, sourcePath + "\\" + fileName, true);

            if (Directory.Exists(destinationPath) == false)

            {
                Directory.CreateDirectory(destinationPath);
            }

            File.Copy(sourceDirectory, destinationDirectory);
            currentFileName = fileName;
            Console.WriteLine(fileName);
            return fileName;
        }
        public static void CreateFileName(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            string destinationPath = @"C:\Test\Save";
            string[] currentFile = Directory.GetFiles(@"C:\Test\New");

            //Get File Info
            foreach (var file in currentFile)
            {
                FileInfo fileInfo = new FileInfo(file);
                sourcePath = fileInfo.DirectoryName;
                currentFileName = fileInfo.Name;
            }

            //Scenario 1 - Location is different, Plate is different, Same VehicleId            
            if (Scenario == 1)
            {
                CreateVehicleId();
                for (int cameraIndex = 0; cameraIndex < cameras.Length; cameraIndex++)
                {
                    CreateRandomPlate();
                    int randomLocationIndex = random.Next(0, locationCodes.Length);
                    MoveFile(destinationPath, CreateTime(), randomLocationIndex, cameraIndex);
                }
            }
            //Scenario 2 - Location is different, Same Plate, VehicleId is different
            if (Scenario == 2)
            {
                CreateRandomPlate();
                for (int cameraIndex = 0; cameraIndex < cameras.Length; cameraIndex++)
                {
                    CreateVehicleId();
                    int randomLocationIndex = random.Next(0,locationCodes.Length);
                    MoveFile(destinationPath, CreateTime(), randomLocationIndex, cameraIndex) ;
                }
            }       
            timer.Start();
        }   

    }
}
