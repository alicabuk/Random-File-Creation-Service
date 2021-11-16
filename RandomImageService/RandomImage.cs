using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

namespace RandomImageService
{
    public partial class RandomImage : ServiceBase
    {
        Timer timer;

        Random random = new Random();
        string alphabet = "ABCDEFGHIJKLMNOPRSTUVYZ";
        string numbers = "0123456789";
        char[] randomPlate = new char[8];
        char[] vehicleId = new char[13];

        string currentFileName;
        string sourcePath;

        string[] cameras = ConfigurationManager.AppSettings["Cameras"].Split(',');
        string[] locationCodes = ConfigurationManager.AppSettings["LocationCodes"].Split(',');
        public RandomImage()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer
            {
                Interval = 3000,
                AutoReset = true
            };
            timer.Elapsed += FileNameByScenario;
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer.Dispose();
        }

        public void CreateRandomPlate()
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
        public void CreateVehicleId()
        {
            for (int i = 0; i < vehicleId.Length; i++)
            {
                vehicleId[i] = numbers[random.Next(numbers.Length)];
            }
        }
        public string CreateTime()
        {
            DateTime date = DateTime.Now;
            date = date.AddMinutes(int.Parse(ConfigurationManager.AppSettings["MinuteDuration"]));
            string time = date.ToString("yyyy-MM-dd-HH-mm-ss-fff");

            return time;
        }
        public string CreatePlateLocation()
        {
            int branchSelection = int.Parse(ConfigurationManager.AppSettings["BranchSelection"]);
            string plateLocation =string.Empty;
            string colors = "0123456";
            string types = "012";
            string directions = "0123";

            if (branchSelection == 1)
            {
                plateLocation = "973-920-1156-960";
            }
            if (branchSelection == 2)
            {
                plateLocation = string.Concat(colors[random.Next(colors.Length)]+"-"+ types[random.Next(types.Length)] + "-" + directions[random.Next(directions.Length)] + "-" + "0");
            }

            return plateLocation;
        }
        public string CreateFileNameAndMove(string destinationPath, string time, int locationIndex, int cameraIndex)
        {
            System.Threading.Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["ImageRenderingTime"]));
            string newPlate = new string(randomPlate);
            string newVehicleId = new string(vehicleId);
            string fileExtension = ConfigurationManager.AppSettings["FileExtension"];

            var fileName = string.Concat(newPlate + "_" + cameras[cameraIndex] + "_" + newVehicleId + "_" + locationCodes[locationIndex] + "_" + time + "_" + CreatePlateLocation() + fileExtension);

            string sourceDirectory = Path.Combine(sourcePath, fileName);
            string destinationDirectory = Path.Combine(destinationPath, fileName);

            File.Move(sourcePath + "\\" + currentFileName, sourcePath + "\\" + fileName);

            if (Directory.Exists(destinationPath) == false)
            {
                Directory.CreateDirectory(destinationPath);
            }

            File.Copy(sourceDirectory, destinationDirectory);
            currentFileName = fileName;

            return fileName;
        }
        public void FileNameByScenario(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            int scenario = int.Parse(ConfigurationManager.AppSettings["Scenario"]);
            string destinationPath = ConfigurationManager.AppSettings["DestinationPath"];
            string[] currentFile = Directory.GetFiles(ConfigurationManager.AppSettings["SourcePath"]);

            foreach (var file in currentFile)
            {
                FileInfo fileInfo = new FileInfo(file);
                sourcePath = fileInfo.DirectoryName;
                currentFileName = fileInfo.Name;
            }

            //Scenario 1 - Location(BIOS ID) is different, Plate is different, Same Vehi"cleId(Unique)            
            if (scenario == 1)
            {
                CreateVehicleId();
                for (int cameraIndex = 0; cameraIndex < cameras.Length; cameraIndex++)
                {
                    CreateRandomPlate();
                    int randomLocationIndex = random.Next(0, locationCodes.Length);
                    CreateFileNameAndMove(destinationPath, CreateTime(), randomLocationIndex, cameraIndex);
                }
            }
            //Scenario 2 - Location(BIOS ID) is different, Same Plate, VehicleId is different(Unique)
            if (scenario == 2)
            {
                CreateRandomPlate();
                for (int cameraIndex = 0; cameraIndex < cameras.Length; cameraIndex++)
                {
                    CreateVehicleId();
                    int randomLocationIndex = random.Next(0, locationCodes.Length);
                    CreateFileNameAndMove(destinationPath, CreateTime(), randomLocationIndex, cameraIndex);
                }
            }
            timer.Start();
        }
    }
}
