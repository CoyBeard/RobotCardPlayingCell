using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using DirectShowLib;
using System.IO.Ports;
using Emgu.CV.Util;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Microsoft.ML;
using Card_Playing_Robot_Cell;
using System.Timers;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Imaging;
using System.Xml.Linq;
using Tesseract;
using IronOcr;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Tensorflow;
using BitMiracle.LibTiff.Classic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;
using System.Data.SqlClient;
using System.Collections;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using NAudio.Wave;
using NModbus;
using NModbus.IO;
using NModbus.Serial;
using NModbus.Utility;
using Emgu.CV.Features2D;
using Microsoft.ML.Data;
using Emgu.CV.Flann;
using System.Data.SqlTypes;
using System.Security.Cryptography;
//using IronSoftware.Drawing;
//using IronSoftware.Drawing;
//using SixLabors.ImageSharp;
//using IronSoftware.Drawing;

namespace Card_Playing_Robot_Cell
{

    public partial class Form1 : Form
    {
        //TESTING
        bool EnableIPIOs = true;



        // Define the Modbus server's IP address and port
        static string IPIO1IP = "192.168.0.199"; // Replace with your server's IP address
        static int IPIO1Port = 502; // Modbus default port is 4196
        static string IPIO12P = "192.168.0.200"; // Replace with your server's IP address
        static int IPIO2Port = 502; // Modbus default port is 4196
        static string IPIO3IP = "192.168.0.19";
        static int IPIO3Port = 502;
        static IModbusMaster IPIO3;

        // Specify the slave ID (unit ID) of the device you want to communicate with
        static byte IPIO3SlaveID = 1;  // Modify this according to your device configuration

        // Create a TCP client socket
        static TcpClient IPIO1;
        static TcpClient IPIO2;
        // Get the network stream for sending data
        NetworkStream IPIO1Stream;
        NetworkStream IPIO2Stream;

        // byte arrays to send
        List<byte[]> OutCodes = new List<byte[]>();
        byte[] R1ON = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8C, 0x3A };    //snd 0     16
        byte[] R1OFF = new byte[] { 0x01, 0x05, 0x00, 0x00, 0x00, 0x00, 0xCD, 0xCA };   //snd 1     17
        byte[] R2ON = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA };    //snd 2     18
        byte[] R2OFF = new byte[] { 0x01, 0x05, 0x00, 0x01, 0x00, 0x00, 0x9C, 0x0A };   //snd 3     19
        byte[] R3ON = new byte[] { 0x01, 0x05, 0x00, 0x02, 0xFF, 0x00, 0x2D, 0xFA };    //snd 4     20
        byte[] R3OFF = new byte[] { 0x01, 0x05, 0x00, 0x02, 0x00, 0x00, 0x6C, 0x0A };   //snd 5     21
        byte[] R4ON = new byte[] { 0x01, 0x05, 0x00, 0x03, 0xFF, 0x00, 0x7C, 0x3A };    //snd 6     22
        byte[] R4OFF = new byte[] { 0x01, 0x05, 0x00, 0x03, 0x00, 0x00, 0x3D, 0xCA };   //snd 7     23
        byte[] R5ON = new byte[] { 0x01, 0x05, 0x00, 0x04, 0xFF, 0x00, 0xCD, 0xFB };    //snd 8     24
        byte[] R5OFF = new byte[] { 0x01, 0x05, 0x00, 0x04, 0x00, 0x00, 0x8C, 0x0B };   //snd 9     25
        byte[] R6ON = new byte[] { 0x01, 0x05, 0x00, 0x05, 0xFF, 0x00, 0x9C, 0x3B };    //snd 10    26
        byte[] R6OFF = new byte[] { 0x01, 0x05, 0x00, 0x05, 0x00, 0x00, 0xDD, 0xCB };   //snd 11    27  
        byte[] R7ON = new byte[] { 0x01, 0x05, 0x00, 0x06, 0xFF, 0x00, 0x6C, 0x3B };    //snd 12    28
        byte[] R7OFF = new byte[] { 0x01, 0x05, 0x00, 0x06, 0x00, 0x00, 0x2D, 0xCB };   //snd 13    29
        byte[] R8ON = new byte[] { 0x01, 0x05, 0x00, 0x07, 0xFF, 0x00, 0x3D, 0xFB };    //snd 14    30
        byte[] R8OFF = new byte[] { 0x01, 0x05, 0x00, 0x07, 0x00, 0x00, 0x7C, 0x0B };   //snd 15    31    
        byte[] RALLON = new byte[] { 0x01, 0x05, 0x00, 0xFF, 0xFF, 0x00, 0xBC, 0x0A };  //snd 32        
        byte[] RALLOFF = new byte[] { 0x01, 0x05, 0x00, 0xFF, 0x00, 0x00, 0xFD, 0xFA }; //snd 33
                                                                                        //
        string[] OutputDesignators = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                       "Draw 2", "Reverse", "Skip", "UNO", "Wild", "Wild Draw 4", }; //Index = Output Number

        string[] UNOCards = { "Blue 0", "Blue 1", "Blue 2", "Blue 3", "Blue 4", "Blue 5", "Blue 6", "Blue 7", "Blue 8", "Blue 9",
                              "Green 0", "Green 1", "Green 2", "Green 3", "Green 4", "Green 5", "Green 6", "Green 7", "Green 8", "Green 9",
                              "Red 0", "Red 1", "Red 2", "Red 3", "Red 4", "Red 5", "Red 6", "Red 7", "Red 8", "Red 9",
                              "Yellow 0", "Yellow 1", "Yellow 2", "Yellow 3", "Yellow 4", "Yellow 5", "Yellow 6", "Yellow 7", "Yellow 8", "Yellow 9",
                              "Blue Draw 2", "Blue Reverse", "Blue Skip", "Green Draw 2", "Green Reverse", "Green Skip",
                              "Red Draw 2", "Red Reverse", "Red Skip", "Yellow Draw 2", "Yellow Reverse", "Yellow Skip", 
                              "UNO", "Wild", "Wild Draw 4", "Blue", "Green", "Red", "Yellow", "Terminator"}; //Index = Card ID

        string[] UNOCardsNoColor = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                              "Draw 2", "Reverse", "Skip", "UNO", "Wild", "Wild Draw 4", }; //Index = Card ID

        //List of all playable scenario 
        List<List<int>> CardVCard = new List<List<int>>();
        List<int> Card0 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 41, 42, 55 };
        List<int> Card1 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 21, 31, 40, 41, 42, 55 };
        List<int> Card2 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 22, 32, 40, 41, 42, 55 };
        List<int> Card3 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13, 23, 33, 40, 41, 42, 55 };
        List<int> Card4 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 14, 24, 34, 40, 41, 42, 55 };
        List<int> Card5 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 15, 25, 35, 40, 41, 42, 55 };
        List<int> Card6 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 16, 26, 36, 40, 41, 42, 55 };
        List<int> Card7 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 17, 27, 37, 40, 41, 42, 55 };
        List<int> Card8 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 18, 28, 38, 40, 41, 42, 55 };
        List<int> Card9 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 19, 29, 39, 40, 41, 42, 55 };
        List<int> Card10 = new List<int> { 0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 30, 43, 44, 45, 56 };
        List<int> Card11 = new List<int> { 1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 21, 31, 43, 44, 45, 56 };
        List<int> Card12 = new List<int> { 2, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 22, 32, 43, 44, 45, 56 };
        List<int> Card13 = new List<int> { 3, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 23, 33, 43, 44, 45, 56 };
        List<int> Card14 = new List<int> { 4, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 24, 34, 43, 44, 45, 56 };
        List<int> Card15 = new List<int> { 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 25, 35, 43, 44, 45, 56 };
        List<int> Card16 = new List<int> { 6, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 26, 36, 43, 44, 45, 56 };
        List<int> Card17 = new List<int> { 7, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 27, 37, 43, 44, 45, 56 };
        List<int> Card18 = new List<int> { 8, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 28, 38, 43, 44, 45, 56 };
        List<int> Card19 = new List<int> { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 29, 39, 43, 44, 45, 56 };
        List<int> Card20 = new List<int> { 0, 10, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 46, 47, 48, 57 };
        List<int> Card21 = new List<int> { 1, 11, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 31, 46, 47, 48, 57 };
        List<int> Card22 = new List<int> { 2, 12, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 32, 46, 47, 48, 57 };
        List<int> Card23 = new List<int> { 3, 13, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 33, 46, 47, 48, 57 };
        List<int> Card24 = new List<int> { 4, 14, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 34, 46, 47, 48, 57 };
        List<int> Card25 = new List<int> { 5, 15, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 35, 46, 47, 48, 57 };
        List<int> Card26 = new List<int> { 6, 16, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 36, 46, 47, 48, 57 };
        List<int> Card27 = new List<int> { 7, 17, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 37, 46, 47, 48, 57 };
        List<int> Card28 = new List<int> { 8, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 38, 46, 47, 48, 57 };
        List<int> Card29 = new List<int> { 9, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 39, 46, 47, 48, 57 };
        List<int> Card30 = new List<int> { 0, 10, 20, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card31 = new List<int> { 1, 11, 21, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card32 = new List<int> { 2, 12, 22, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card33 = new List<int> { 3, 13, 23, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card34 = new List<int> { 4, 14, 24, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card35 = new List<int> { 5, 15, 25, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card36 = new List<int> { 6, 16, 26, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card37 = new List<int> { 7, 17, 27, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card38 = new List<int> { 8, 18, 28, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card39 = new List<int> { 9, 19, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 58 };
        List<int> Card40 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 40, 41, 42, 43, 46, 49, 55 };
        List<int> Card41 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 40, 41, 42, 44, 47, 50, 55 };
        List<int> Card42 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 40, 41, 42, 45, 48, 51, 55 };
        List<int> Card43 = new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 40, 43, 44, 45, 46, 49, 56 };
        List<int> Card44 = new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 41, 43, 44, 45, 47, 50, 56 };
        List<int> Card45 = new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 42, 43, 44, 45, 48, 51, 56 };
        List<int> Card46 = new List<int> { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 40, 43, 46, 47, 48, 49, 57 };
        List<int> Card47 = new List<int> { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 41, 44, 46, 47, 48, 50, 57 };
        List<int> Card48 = new List<int> { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 42, 45, 46, 47, 48, 51, 57 };
        List<int> Card49 = new List<int> { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 43, 46, 49, 50, 51, 58 };
        List<int> Card50 = new List<int> { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 41, 44, 47, 49, 50, 51, 58 };
        List<int> Card51 = new List<int> { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 42, 45, 48, 49, 50, 51, 58 };
        List<int> Card52 = new List<int> { 52 };
        List<int> Card53 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 
                                           30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 55, 56, 57, 58 };
        List<int> Card54 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
                                           30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 55, 56, 57, 58 };
        List<int> Card55 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 40, 41, 42, 53, 54, 55 };
        List<int> Card56 = new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 43, 44, 45, 53, 54, 56 };
        List<int> Card57 = new List<int> { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 46, 47, 48, 53, 54, 57 };
        List<int> Card58 = new List<int> { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 49, 50, 51, 53, 54, 58 };
        List<int> Card59 = new List<int> { };


        string CardIconFilePath = @"C:\Users\Coy\source\repos\Card Playing Robot Cell - 22\Card Playing Robot Cell\UNO Card Icons";
        string CardIconExtention = ".png";

        static int StartingCardsCount = 7;//7

        

        static string[] PlaceFirstCardInstructions = new string[]
    {
        $"Player, it's your move. Place the initial card from the draw pile.",
        $"It's your turn, human. Go ahead and lay down the first card from the draw pile.",
        $"Human, the move is yours. Play the first card from the draw pile.",
        $"Your move, player. Place the opening card from the draw pile.",
        $"It's time for your move, human. Lay down the initial card from the draw pile.",
        $"Player, go ahead and make your move by placing the first card from the draw pile.",
        $"Human player, it's your turn. Place the starting card from the draw pile.",
        $"It's your move, human. Go ahead and play the first card from the draw pile.",
        $"Player, the move is yours. Place the first card from the draw pile.",
        $"Human, go ahead and make your move by playing the initial card from the draw pile.",
        $"Your turn, player. Lay down the first card from the draw pile.",
        $"It's up to you, human. Place the first card from the draw pile.",
        $"Player, it's your move now. Play the opening card from the draw pile.",
        $"Human, go ahead and make your move by placing the first card from the draw pile.",
        $"Your move, player. Lay down the initial card from the draw pile.",
        $"It's your turn, human. Play the first card from the draw pile.",
        $"Player, the move is yours. Place the starting card from the draw pile.",
        $"Human player, it's your turn. Play the first card from the draw pile.",
        $"It's your move, human. Go ahead and place the first card from the draw pile.",
        $"Player, go ahead and make your move by playing the initial card from the draw pile."
    };

        static string[] CardColorPrompts = new string[]
    {
        "What color did you choose for your wild card?",
        "Could you let us know the color of your chosen wild card?",
        "Which color did you pick for your wild card?",
        "Tell us the color you selected for your wild card.",
        "What's the color of the wild card you decided on?",
        "Mind sharing the color you assigned to your wild card?",
        "Please specify the color for your wild card.",
        "We're curious, what's the color of your chosen wild card?",
        "Let us know the color you've set for your wild card.",
        "What did you choose as the color for your wild card?",
        "Inform us about the color you've chosen for your wild card.",
        "Which hue did you pick for your wild card?",
        "Share with us the color of your selected wild card.",
        "Curious to know, what color is your wild card?",
        "Could you inform us of the color you've chosen for your wild card?",
        "What's the color of your wild card choice?",
        "Please reveal the color you've assigned to your wild card.",
        "Mind telling us the color you've decided for your wild card?",
        "Let us in on the secret: what color is your wild card?",
        "Inform the group about the color you've chosen for your wild card."
    };

        public string[] ChosenColorVerificationMessages;

        static string[] MisunderstoodColorResponce = new string[]
    {
        "My mistake! What color should it be?",
        "Oops, my bad. Which color did you mean?",
        "My fault! What's the correct color?",
        "I messed up. What color is it supposed to be?",
        "Oops! Which color were you thinking?",
        "My bad. What's the right color?",
        "I goofed. What color were you going for?",
        "Sorry! Which color did you intend?",
        "My error. What's the correct color?",
        "Oops, my mistake. What color is it?",
        "I misunderstood. Which color is right?",
        "My fault! What color did you have in mind?",
        "I got it wrong. Which color should it be?",
        "Oops! What's the correct color?",
        "Sorry about that. What color is it?",
        "My bad. Which color were you thinking?",
        "I messed up. What's the right color?",
        "Oops! What color were you going for?",
        "My mistake. What's the intended color?",
        "I goofed. Which color is it supposed to be?"
    };


        //Setup Camera Comunications
        VideoCapture capture;
        private List<DsDevice> cameraList;
        //Live Frame
        Mat frame = new Mat();
        //PlayingAreaROI
        Mat PlayingAreaView = new Mat();
        Mat DiscardPileAreaView = new Mat();

        //Set Discard Pile ROI
        // Define the ROI coordinates
        //Whole Card
        Rectangle PlayingAreaROI = new Rectangle(1000, 800, 2400, 1350);
        Rectangle DiscardPileAreaROI = new Rectangle(1900, 1000, 600, 400);
        Rectangle DiscardPileCardROI = new Rectangle(1200, 260, 270, 350);
        Rectangle Player1HandCardROI = new Rectangle(600, 300, 270, 350);
        Rectangle Player2HandCardROI = new Rectangle(1725, 600, 270, 350);
        Rectangle Player3HandCardROI = new Rectangle(925, 850, 270, 350);

        //TEMP Auto Crop
        //Zoomed Center Symbol
        //Rectangle CropNewROI = new Rectangle(67, 87, 135 - 10, 175 - 12);
        //Zoomed Lower Right Symbol
        Rectangle CropNewROI = new Rectangle(140, 215, 85, 105);

        //Zoomed Center Symbol
        Rectangle DiscardPileCenterCardROI = new Rectangle(1267 + 7, 347 + 6, 135 - 3 - 7, 175 - 6 - 6);
        Rectangle Player1HandCenterCardROI = new Rectangle(667 + 15, 387 + 14, 135 - 11 - 15, 175 - 6 - 14);
        Rectangle Player2HandCenterCardROI = new Rectangle(1792 - 8, 687 - 5, 135 - 40 + 8, 175 - 27 + 5);
        Rectangle Player3HandCenterCardROI = new Rectangle(992 + 2, 937 + 15 - 6, 135 - 17 - 2, 175 - 15 - 12 + 6);

        //Zoomed Lower Right Hand Symbol
        Rectangle DiscardPileROI = new Rectangle(1340, 475, 85 + 5, 105);
        Rectangle Player1HandROI = new Rectangle(750, 525, 70 + 10, 90);
        Rectangle Player2HandROI = new Rectangle(1855, 795, 75 + 10, 100);
        Rectangle Player3HandROI = new Rectangle(1075, 1075, 70, 90);


        //Defalt Screenshot Save Folder
        SaveFileDialog ScreenshotSaveDialog = new SaveFileDialog();
        private string ScreenshotDefaultFolderPath = @"..\..\Screenshots\";
        //@"\\192.168.0.201\BasementNAS\Projects\230907 Robotic Card Playing Cell\Programing\PC\Screenshots\";//@"C:\Users\Coy\source\repos\Card Playing Robot Cell - 22\Card Playing Robot Cell\ML Card Images BaW Cropped\9\"
        private string ImgFileType = ".png";
        private string CardIconsFolderPath = @"..\..\UNO Card Icons\";

        //Timer
        bool CaptureFrame = false;
        bool TSTTimerElapsed = false;
        bool TimerCardVerificationPGElapsed = false;
        bool TimerProgressBarTicked = false;



        //TEMP Image Labels
        string DiscardPileResultLabel;
        string Player1HandImageResultLabel;
        string Player2HandImageResultLabel;
        string Player3HandImageResultLabel;

        bool PredictCards = false;
        int PredictCardsProgress = 0;

        bool CameraActive = false;
        bool GameLive = false;
        bool GameRunning = true;

        int PlayersTurn;
        bool NPCTauntSugnalSent = false;

        System.Windows.Forms.Timer RemoteChatTimer = new System.Windows.Forms.Timer();

        System.Windows.Forms.Timer HumanPreTurnTakePhotosTimer = new System.Windows.Forms.Timer();

        List<string> PlayerOrder = new List<string> { "FC006N_3", "A550C_1", "Human", "A550_2" };
        List<string> Players = new List<string> { "Human" };
        List<List<string>> PlayerData = new List<List<string>> 
        {

        };

        int PlayerDataTerminator = 59;

        SpeechSynthesizer SystemSpeachSynthesizer = new SpeechSynthesizer();

        private SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

        int GameTurns = 0;
        bool ReadySignalRecieved = false;

        bool ShuffleCardCompleted = false;
        bool DrawCardCompleted = false;
        bool TauntCompleted = true;

        bool PlayerCanTaunt = false;
        bool CardPredicted = false;
        int DrawnPredictedCardID = 0;
        string DrawnPredictedCard = "";
        bool HumanDrawCardsMessageSent = false;
        bool HumanDrawCardsComplete = false;

        int DiscardCardID = 0;
        bool DiscardCardIDFound = false;
        int SelectedTreeNode;

        bool TextBoxRemoteChatFirstClicked = false;


        string RemotePlayerName = "";
        bool DrawButtonEnable = true;

        bool GameDirectionClockwise = true;

        static bool LightScreenSawTurn = false;
        bool CardPlayabilityFound = false;
        static int RemotePlayerSelectedCardNullValue = 200;//200 means null
        static int RemotePlayerSelectedCardIndex = RemotePlayerSelectedCardNullValue;
        bool RomotePlayCard = false;
        //bool RemoteTurnOver = false;
        bool RemoteShuffelSigSent = false;

        bool ReadLightScreenStartTimeSet = false;
        bool ReadLightScreenLightScreenWasBlocked = false;

        bool RemotePlayerDrewCardPlayableCheck = false;
        bool RobotRetrievingCard = false;
        bool RobotPlayingCard = false;

        int LastSelectedManageTreeViewIndex = 0;
        bool PlayerIgnoreCardConsequenses = false; //only allow the effects of a specal card on the next player toggle to prevent infinite loop

        BackgroundSubtractorMOG2 backgroundSubtractor;
        bool HumanMotionDetectorVarsReset = false;
        int HumanMotionCoolDownTime = 2500;//time wait to watch motion memory to see if human played or drew card, ms

        bool FirstCardPlayed = false;//first card for the discard pile at the beginning of the game 
        bool HumanTurnEndCall = false;
        bool RemotePlayerTurnEndCall = false;
        bool NPCRobotTurnEndCall = false;

        bool HumanSendPlayFirstCardInstruction = false;
        bool SpecialCardLogicCleared = false;
        int DrawnCards;

        bool HumanChooseColorAskState = true;
        bool HumanChooseColorWaitForResponceState = true;
        bool HumanChooseColorRepeatColorChosenState = false;
        bool HumanChooseColorWaitForConformationState = false;
        bool HumanChooseColorExicuteResponceState = false;
        bool HumanChooseColorRepeatCorrectResponceState = false;
        bool HumanChooseColorPromptCorrectResponceState = false;


        int HumanChooseColorWaitForConformationTime = 8000;
        int HumanChooseColorWaitForConformationStartTime = 0;

        bool SystemSpeachSynthesizerComplete = true;
        bool RemotePlayerDrawingTwoOrFour = false;

        static string HumanChooseColor = "Blank";
        int DiscardPileMotionDrawThreshold = 10; //Threshold for the amount of conturs found after human takes turn, Higher than thresh = Played card, lower = Drawn Card
        bool PlayerDrawingTwoOrFoInitializeVars = false;
        bool PlayerSeeWildCardInitializeVars = false;
        bool HumanTurnInitializeVars = false;
        bool HumanChooseColorWaitForConformationStateInitializeVars = false;
        bool RemotePlayerPlayingFirstCardInitializeVars = false;
        bool PredictCardFinished = false;
        bool CardPlayabilityFoundInitializeVars = false;
        bool UpdatePlayableCardsInitializeVars = false;

        bool RemotePlayerDrawFourAskColorStep = false;
        bool PreviousPlayerPlayedCard = false;
        bool ShuffleCardRunning = false;

        bool PlayedCardColorChosen = false;
        bool PlayedCardColorChosenInitVars = false;

        bool PlayerCanPlayCard = true;
        bool PlayerPlayedCardIDFound = false;
        int PlayerCardPlayingID = 0;
        
        int InitDiscardID = 0;

        bool RemotePlayerPlayedFirstWildCard = false;//bool for when the player plays their first wild card they recieve in instructional prompt

        bool NPCPlayingFirstCardInitializeVars = false;
        bool NPCShuffelSigSent = false;
        bool NPCDrawCardSent = false;

        bool NPCCheckCardPlayability = true;
        bool NPCRobotDrewCardAlready = false;
        int PlayerCardPlayingIndex = 0;
        string NPCWildCardColorChoice = "Blue";

        bool SkipNextPlayerOnAdvanceTurn = false;

        Mat HumanTurnPreImage = new Mat();
        Mat HumanTurnPostImage = new Mat();

        int HumanPlayCardDetectionImageComparisonThreshold = 160; //100 210

        private static Random RandomRunTaunt = new Random();

        bool HumanTurnPromptSent = false;

        private static List<int> InpOverideIPIO3 = new List<int> { 0, 0, 0, 0 };

        bool HumanCardCountAdded = false;
        bool CheckForUno = false;

        bool HumanPreTurnTableStable = false;
        bool HumanPreTurnTakenPhotos = false;

        public Form1()
        {
            InitializeComponent();
            InitializeMessageTimer();
            Application.Idle += new EventHandler(PlayGame);
            SystemSpeachSynthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
        }
        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {// The speech synthesis has completed
            SystemSpeachSynthesizerComplete = true;
            Console.WriteLine("Speech has finished");
            //ChatBuffer.RemoveAt(0);
            //Console.WriteLine("ChatBuffer:");
            //PrintListOfStringLists(ChatBuffer);
            //check buffer to speek another chat
            SendChatReadBuffer();
        }
        private void InitializeMessageTimer()
        {
            RemoteChatTimer.Interval = 3000;  // Set the display duration in milliseconds (3 seconds in this example)
            RemoteChatTimer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            labelManageChat.Text = string.Empty;
            labelManageChatRemoteUser.Text = string.Empty;
            labelRemoteChat.Text = string.Empty;
            labelRemoteChatSystem.Text = string.Empty;
            RemoteChatTimer.Stop();
            //check if theres more in the chat buffer
            SendChatReadBuffer();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Installed voices -");
            foreach (InstalledVoice voice in SystemSpeachSynthesizer.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                Console.WriteLine(" Voice Name: " + info.Name + info.Culture + info.Description);
            }

            OutCodes.Add(R1OFF); OutCodes.Add(R1ON); OutCodes.Add(R2OFF); OutCodes.Add(R2ON); OutCodes.Add(R3OFF); OutCodes.Add(R3ON);
            OutCodes.Add(R4OFF); OutCodes.Add(R4ON); OutCodes.Add(R5OFF); OutCodes.Add(R5ON); OutCodes.Add(R6OFF); OutCodes.Add(R6ON);
            OutCodes.Add(R7OFF); OutCodes.Add(R7ON); OutCodes.Add(R8OFF); OutCodes.Add(R8ON); OutCodes.Add(RALLOFF); OutCodes.Add(RALLON);

            CardVCard.add(Card0); CardVCard.add(Card1); CardVCard.add(Card2); CardVCard.add(Card3); CardVCard.add(Card4);
            CardVCard.add(Card5); CardVCard.add(Card6); CardVCard.add(Card7); CardVCard.add(Card8); CardVCard.add(Card9);
            CardVCard.add(Card10); CardVCard.add(Card11); CardVCard.add(Card12); CardVCard.add(Card13); CardVCard.add(Card14);
            CardVCard.add(Card15); CardVCard.add(Card16); CardVCard.add(Card17); CardVCard.add(Card18); CardVCard.add(Card19);
            CardVCard.add(Card20); CardVCard.add(Card21); CardVCard.add(Card22); CardVCard.add(Card23); CardVCard.add(Card24);
            CardVCard.add(Card25); CardVCard.add(Card26); CardVCard.add(Card27); CardVCard.add(Card28); CardVCard.add(Card29);
            CardVCard.add(Card30); CardVCard.add(Card31); CardVCard.add(Card32); CardVCard.add(Card33); CardVCard.add(Card34);
            CardVCard.add(Card35); CardVCard.add(Card36); CardVCard.add(Card37); CardVCard.add(Card38); CardVCard.add(Card39);
            CardVCard.add(Card40); CardVCard.add(Card41); CardVCard.add(Card42); CardVCard.add(Card43); CardVCard.add(Card44);
            CardVCard.add(Card45); CardVCard.add(Card46); CardVCard.add(Card47); CardVCard.add(Card48); CardVCard.add(Card49);
            CardVCard.add(Card50); CardVCard.add(Card51); CardVCard.add(Card52); CardVCard.add(Card53); CardVCard.add(Card54);
            CardVCard.add(Card55); CardVCard.add(Card56); CardVCard.add(Card57); CardVCard.add(Card58); CardVCard.add(Card59);
            //Cnnect to IP IOs
            if (EnableIPIOs)
            {
                try
                {
                    // Create a TCP client socket
                    IPIO1 = new TcpClient(IPIO1IP, IPIO1Port);
                    IPIO2 = new TcpClient(IPIO12P, IPIO2Port);
                    // Get the network stream for sending data
                    IPIO1Stream = IPIO1.GetStream();
                    IPIO2Stream = IPIO2.GetStream();

                    // Create a Modbus TCP master
                    TcpClient client = new TcpClient(IPIO3IP, IPIO3Port);
                    ModbusFactory factory = new ModbusFactory();
                    IPIO3 = factory.CreateMaster(client);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            

            // Initialize the list of available cameras
            cameraList = GetAvailableCameras();

            //Set defalt screenshot folder directory
            if (!Directory.Exists(ScreenshotDefaultFolderPath))
            {
                Directory.CreateDirectory(ScreenshotDefaultFolderPath);
            }

            // Populate the combo box with available cameras
            foreach (var camera in cameraList)
            {
                CbCameraSelect.Items.Add(camera.Name);
            }

            //_prevFrame = new Mat();
            backgroundSubtractor = new BackgroundSubtractorMOG2();

            labelManageChat.Text = string.Empty;
            labelManageChatRemoteUser.Text = string.Empty;
            labelRemoteChat.Text = string.Empty;
            labelRemoteChatSystem.Text = string.Empty;

            // Load a built-in or custom grammar
            Choices commands = new Choices("Red", "Green", "Blue", "Yellow", "Uno", "No", "wait", "stop", "yes", "yeah", "yup");//, "Pause", "Okay"
            GrammarBuilder grammarBuilder = new GrammarBuilder(commands);
            Grammar grammar = new Grammar(grammarBuilder);

            // Set the loaded grammar to the recognizer
            recognizer.LoadGrammar(grammar);
            //recognizer.LoadGrammar(new DictationGrammar());

            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;


            // Create a timer with a 1000ms (1 second) interval
            System.Timers.Timer timer = new System.Timers.Timer(10);//500

            // Create a timer with a 1000ms (1 second) interval
            System.Timers.Timer TSTtimer = new System.Timers.Timer(2000);

            System.Timers.Timer timerCardVerificationPG = new System.Timers.Timer(100);


            timer.Elapsed += TimerElapsed;
            TSTtimer.Elapsed += TSTtimerElapsed;
            timerCardVerificationPG.Elapsed += timerCardVerificationPGElapsedCall;

            // Start the timer
            timer.Start();
            TSTtimer.Start();
            timerCardVerificationPG.Start();

            HumanPreTurnTakePhotosTimer.Interval = 800;  // Set the display duration in milliseconds between sample photos
            HumanPreTurnTakePhotosTimer.Tick += HumanPreTurnTakePhotosTimer_Tick;
            HumanPreTurnTakePhotosTimer.Start();

        }

        List<Mat> HumanPreTurnPhotos = new List<Mat>();
        private void HumanPreTurnTakePhotosTimer_Tick(object sender, EventArgs e)
        {
            if (HumanPreTurnTakenPhotos)
            {
                if (HumanPreTurnPhotos.Count <= 3)
                {
                    //take three photos
                    Console.WriteLine("HumanPreTurnTakenPhotos Take Photo");

                    //record Discard pile before turn to compare to after turn for draw detection
                    Bitmap DiscardPileImageFull = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
                    Graphics.FromImage(DiscardPileImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);

                    // Convert the Bitmap to an Image<Bgr, Byte>
                    HumanPreTurnPhotos.add(DiscardPileImageFull.ToMat());
                }
                if (HumanPreTurnPhotos.Count >= 3)
                {
                    //if we took three photos
                    int intValue = AreImagesDifferent(HumanPreTurnPhotos[0], HumanPreTurnPhotos[1], HumanPlayCardDetectionImageComparisonThreshold) ? 0 : 1;
                    Console.WriteLine($"Image 0 and 1 = {intValue}");
                    intValue += AreImagesDifferent(HumanPreTurnPhotos[1], HumanPreTurnPhotos[2], HumanPlayCardDetectionImageComparisonThreshold) ? 0 : 1;
                    Console.WriteLine($"Image 0 and 1 and 1 and 2 = {intValue}");
                    intValue += AreImagesDifferent(HumanPreTurnPhotos[0], HumanPreTurnPhotos[2], HumanPlayCardDetectionImageComparisonThreshold) ? 0 : 1;
                    Console.WriteLine($"Image 0 and 1 and 1 and 2 and 0 and 2 = {intValue}");

                    HumanPreTurnPhotos.Clear();

                    if (intValue == 3)
                    {
                        //all three images are the same
                        Console.WriteLine($"all three images are the same");

                        HumanPreTurnTakenPhotos = false;
                        HumanPreTurnTableStable = true;
                    }
                    //else repeat
                    Console.WriteLine($"Redo Search");
                }
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            CaptureFrame = true;
        }
        private void TSTtimerElapsed(object sender, ElapsedEventArgs e)
        {
            TSTTimerElapsed = true;
        }
        private void timerCardVerificationPGElapsedCall(object sender, ElapsedEventArgs e)
        {
            TimerProgressBarTicked = true;
            //Console.WriteLine("Tick");
        }
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;
            //RemoteChatSend(recognizedText, "IRL Game");
            Console.WriteLine(recognizedText);

            if (e.Result.Text == "okay" && SystemSpeachSynthesizerComplete)
            {
                HumanDrawCardsComplete = true;
            }
            if (HumanChooseColorWaitForResponceState && SystemSpeachSynthesizerComplete)//wild card human collor indicating interface 
            {
                if (e.Result.Text == "Red")
                {
                    Console.WriteLine("Player Chose Red");
                    HumanChooseColor = "Red";
                    HumanChooseColorWaitForResponceState = false;
                    HumanChooseColorRepeatColorChosenState = true;
                }
                if (e.Result.Text == "Green")
                {
                    Console.WriteLine("Player Chose Green");
                    HumanChooseColor = "Green";
                    HumanChooseColorWaitForResponceState = false;
                    HumanChooseColorRepeatColorChosenState = true;
                }
                if (e.Result.Text == "Blue")
                {
                    Console.WriteLine("Player Chose Blue");
                    HumanChooseColor = "Blue";
                    HumanChooseColorWaitForResponceState = false;
                    HumanChooseColorRepeatColorChosenState = true;
                }
                if (e.Result.Text == "Yellow")
                {
                    Console.WriteLine("Player Chose Yellow");
                    HumanChooseColor = "Yellow";
                    HumanChooseColorWaitForResponceState = false;
                    HumanChooseColorRepeatColorChosenState = true;
                }
            }
            if (HumanChooseColorWaitForConformationState && SystemSpeachSynthesizerComplete)
            {
                if (e.Result.Text == "No" || e.Result.Text == "wait" || e.Result.Text == "stop")
                {
                    Console.WriteLine("Player Cancled");
                    HumanChooseColorWaitForConformationState = false;
                    HumanChooseColorPromptCorrectResponceState = true;
                }
            }
            
        }//Choices commands = new Choices("Red", "Green", "Blue", "Yellow", "Uno", "No", "wait", "stop", "okay");//, "Pause"

        private void PictureBox_Click(object sender, EventArgs e)
        {
            // This event handler will be called when any PictureBox is clicked

            if (sender is PictureBox pictureBox)
            {
                // Get the index of the clicked PictureBox in its parent's controls collection
                RemotePlayerSelectedCardIndex = pictureBox.Parent.Controls.IndexOf(pictureBox);

                UpdatePlayerDataDependants(true);//reset card graphics
                //CardPlayabilityFound = false;//redraw dimmed cards
                ChangeImageAt(flowLayoutPanelPlayersCards, RemotePlayerSelectedCardIndex, AddYellowBorder(GetImageAt(flowLayoutPanelPlayersCards, RemotePlayerSelectedCardIndex), 3));

                // Print the index
                Console.WriteLine($"RemotePlayerSelectedCard = {RemotePlayerSelectedCardIndex}.");

                
            }
        }

        //Live View Camera
        private List<DsDevice> GetAvailableCameras()
        {
            List<DsDevice> cameras = new List<DsDevice>();

            // Enumerate video input devices using DirectShowLib
            DsDevice[] devices = DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);
            foreach (DsDevice device in devices)
            {
                cameras.Add(device);
            }

            return cameras;
        }

        private int MotionInsideDicardPile = 0;
        private int MotionOutsideDiscardPile = 0;
        private async void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            if (CaptureFrame)
            {
                // Retrieve the latest frame from the video capture
                capture.Retrieve(frame);

                PlayingAreaView = new Mat(frame, PlayingAreaROI);

                DiscardPileAreaView = new Mat(frame, DiscardPileAreaROI);

                pictureBoxDiscardPile.Image = DiscardPileAreaView.ToBitmap();

                comboBoxRemotePlayerSelection.Invoke(new Action(() =>
                {
                    if (comboBoxRemotePlayerSelection.Text != null)
                    {
                        if (comboBoxRemotePlayerSelection.Text == "A550C_1")
                        {
                            pictureBoxPlayerHand.Image = new Mat(PlayingAreaView, Player1HandCardROI).ToBitmap();
                        }
                        if (comboBoxRemotePlayerSelection.Text == "A550_2")
                        {
                            pictureBoxPlayerHand.Image = new Mat(PlayingAreaView, Player2HandCardROI).ToBitmap();
                        }
                        if (comboBoxRemotePlayerSelection.Text == "FC006N_3")
                        {
                            pictureBoxPlayerHand.Image = new Mat(PlayingAreaView, Player3HandCardROI).ToBitmap();
                        }
                    }
                }));

                //draw Discard Pile ROI
                CvInvoke.Rectangle(PlayingAreaView, DiscardPileROI, new MCvScalar(0, 0, 0), 2);
                CvInvoke.Rectangle(PlayingAreaView, DiscardPileCardROI, new MCvScalar(0, 0, 0), 2);
                CvInvoke.PutText(PlayingAreaView, "Discard Pile", new Point(DiscardPileCenterCardROI.X - 80, DiscardPileCenterCardROI.Y - 70), FontFace.HersheySimplex, 1, new MCvScalar(0, 0, 255), 3);

                //draw Player 1 Hand ROI
                CvInvoke.Rectangle(PlayingAreaView, Player1HandROI, new MCvScalar(0, 0, 0), 2);
                //CvInvoke.Rectangle(PlayingAreaView, Player1HandCardROI, new MCvScalar(0, 0, 0), 2);
                CvInvoke.PutText(PlayingAreaView, "Player 1 Hand", new Point(Player1HandCenterCardROI.X - 80, Player1HandCenterCardROI.Y - 70), FontFace.HersheySimplex, 1, new MCvScalar(0, 0, 255), 3);

                //draw Player 2 Hand ROI
                CvInvoke.Rectangle(PlayingAreaView, Player2HandROI, new MCvScalar(0, 0, 0), 2);
                //CvInvoke.Rectangle(PlayingAreaView, Player2HandCardROI, new MCvScalar(0, 0, 0), 2);
                CvInvoke.PutText(PlayingAreaView, "Player 2 Hand", new Point(Player2HandCenterCardROI.X - 80, Player2HandCenterCardROI.Y - 70), FontFace.HersheySimplex, 1, new MCvScalar(0, 0, 255), 3);

                //draw Player 3 Hand ROI
                CvInvoke.Rectangle(PlayingAreaView, Player3HandROI, new MCvScalar(0, 0, 0), 2);
                //CvInvoke.Rectangle(PlayingAreaView, Player3HandCardROI, new MCvScalar(0, 0, 0), 2);
                CvInvoke.PutText(PlayingAreaView, "Player 3 Hand", new Point(Player3HandCenterCardROI.X - 80, Player3HandCenterCardROI.Y - 70), FontFace.HersheySimplex, 1, new MCvScalar(0, 0, 255), 3);

                if (PredictCards)
                {
                    var DiscardPileResult = PredictCard(0); //PredictCard(0);
                    var Player1HandImageResult = PredictCard(1);
                    var Player2HandImageResult = PredictCard(2);
                    var Player3HandImageResult = PredictCard(3);

                    pictureBox1.Image = DiscardPileResult.image;
                    pictureBox2.Image = Player1HandImageResult.image;
                    pictureBox3.Image = Player2HandImageResult.image;
                    pictureBox4.Image = Player3HandImageResult.image;

                    DiscardPileResultLabel = DiscardPileResult.PredictedCard + " " + DiscardPileResult.Confidence + "% Sure";// + DiscardPileResult.Ocr;
                    Player1HandImageResultLabel = Player1HandImageResult.PredictedCard + " " + Player1HandImageResult.Confidence + "% Sure";
                    Player2HandImageResultLabel = Player2HandImageResult.PredictedCard + " " + Player2HandImageResult.Confidence + "% Sure";
                    Player3HandImageResultLabel = Player3HandImageResult.PredictedCard + " " + Player3HandImageResult.Confidence + "% Sure";

                    label1.Invoke(new Action(() =>
                    {
                        label1.Text = DiscardPileResultLabel;
                    }));
                    label2.Invoke(new Action(() =>
                    {
                        label2.Text = Player1HandImageResultLabel;
                    }));
                    label3.Invoke(new Action(() =>
                    {
                        label3.Text = Player2HandImageResultLabel;
                    }));
                    label4.Invoke(new Action(() =>
                    {
                        label4.Text = Player3HandImageResultLabel;
                    }));
                }

                //backgroundSubtractor

                //Mat SmoothFrame = new Mat ();
                //CvInvoke.GaussianBlur(PlayingAreaView, SmoothFrame, new Size(9, 9), 1);

                //Mat foregroundMask = new Mat ();    
                //backgroundSubtractor.Apply(SmoothFrame, foregroundMask);

                //CvInvoke.Threshold(foregroundMask, foregroundMask, 240, 245, ThresholdType.Binary);
                //CvInvoke.MorphologyEx(foregroundMask, foregroundMask, MorphOp.Close, Mat.Ones(3, 3, DepthType.Cv8U, 1), new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(0,0,0));

                //VectorOfVectorOfPoint contures = new VectorOfVectorOfPoint();
                //CvInvoke.FindContours(foregroundMask, contures, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                //int MaxContures = 5;
                //int MinArea = 100;
                //int ConturesCount = 0;
                //for (int i = 0; i < contures.Size; i++)
                //{
                //    var bbox = CvInvoke.BoundingRectangle(contures[i]);
                //    var area = bbox.Width * bbox.Height;
                //    //var ar = (float)bbox.Width / bbox.Height;
                //    if (area > MinArea)// && ConturesCount < MaxContures
                //    {
                //        if (DiscardPileCardROI.Contains(bbox))
                //        {
                //            MotionInsideDicardPile++;
                //            CvInvoke.Rectangle(PlayingAreaView, bbox, new MCvScalar(0, 255, 0), 2);//
                //        }
                //        else
                //        {
                //            //MotionOutsideDiscardPile++;
                //        }
                //        //CvInvoke.Rectangle(PlayingAreaView, bbox, new MCvScalar(0, 255, 0), 2);
                //        //ConturesCount++;
                //    }
                //}



                //Mat PlayingAreaView_Gray = new Mat();   

                //CvInvoke.CvtColor(PlayingAreaView, PlayingAreaView_Gray, ColorConversion.Bgr2Gray);

                //Mat PlayingAreaView_Gray_ROI = new Mat(PlayingAreaView_Gray, DiscardPileCardROI);

                ////_prevFrame
                //if (_prevFrame.IsEmpty)
                //{
                //    PBLiveView.Image = PlayingAreaView.ToBitmap();
                //    _prevFrame = PlayingAreaView_Gray_ROI.Clone();
                //    return;
                //}

                //// Compute absolute difference between current and previous frame
                //CvInvoke.AbsDiff(PlayingAreaView_Gray_ROI, _prevFrame, PlayingAreaView_Gray_ROI);

                //PBLiveView.Image = PlayingAreaView_Gray_ROI.ToBitmap();

                //// Apply a threshold to detect motion
                //CvInvoke.Threshold(PlayingAreaView_Gray_ROI, PlayingAreaView_Gray_ROI, 230, 255, ThresholdType.Binary);

                //// Find contours of the motion
                //VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                //CvInvoke.FindContours(PlayingAreaView_Gray_ROI, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                //// Draw bounding boxes around the contours
                //for (int i = 0; i < contours.Size; i++)
                //{
                //    Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);
                //    CvInvoke.Rectangle(PlayingAreaView, boundingBox, new MCvScalar(0, 255, 0), 2);
                //}

                //PBLiveView.Image = foregroundMask.ToBitmap();

                PBLiveView.Image = PlayingAreaView.ToBitmap();

                // Update the previous frame
                //_prevFrame = PlayingAreaView_Gray_ROI.Clone();

                PredictCards = false;
            }
        }

        private async void PlayGame(object sender, EventArgs e)
        {
            //if (true)//TEMP TESTING
            //{
            //    if (!PlayerIgnoreCardConsequenses)
            //    {//


            //        PlayerIgnoreCardConsequenses = true;
            //    }
            //    if (!LightScreenSawTurn)
            //    {
            //        ReadLightScreen();
            //    }

            //    if (LightScreenSawTurn && !HumanMotionDetectorVarsReset)
            //    {
            //        MotionInsideDicardPile = 0;
            //        MotionOutsideDiscardPile = 0;
            //        int StartTime = Environment.TickCount;
            //        HumanMotionDetectorVarsReset = true;

            //        Console.WriteLine("HumanMotionDetectorVarsReset");
            //    }

            //    //Console.WriteLine($"Environment.TickCount = {Environment.TickCount}, startTime = {startTime}, startTime + HumanMotionCoolDownTime = {startTime * HumanMotionCoolDownTime}");

            //    if (HumanMotionDetectorVarsReset && Environment.TickCount > startTime + HumanMotionCoolDownTime)
            //    {//If motion cool down time is up 
            //        Console.WriteLine("motion cool down time is up");
            //        Console.WriteLine($"MotionInsideDicardPile = {MotionInsideDicardPile}, DiscardPileMotionDrawThreshold = {DiscardPileMotionDrawThreshold}");
            //        if (MotionInsideDicardPile >= DiscardPileMotionDrawThreshold)
            //        {//Player Played Card
            //            Console.WriteLine("Player Played Card");
            //            PlayerIgnoreCardConsequenses = false;
            //        }
            //        else
            //        {//player Drew Card
            //            Console.WriteLine("player Drew Card");
            //            PlayerIgnoreCardConsequenses = true;
            //        }
            //        //turn end
            //        HumanTurnEndCall = true;
            //    }
            //    if (HumanTurnEndCall)
            //    {
            //        //Add to player turn count 
            //        //PlayerData[PlayersTurn][1] = (int.Parse(PlayerData[PlayersTurn][1]) + 1).ToString();
            //        LightScreenSawTurn = false;
            //        HumanMotionDetectorVarsReset = false;
            //        HumanTurnEndCall = false;
            //        HumanSendPlayFirstCardInstruction = false;
            //        //PrintListOfStringLists(PlayerData);
            //        //updatePlayerDataDependants();
            //        //AdvancePlayerTurn();
            //    }
            //}
            //Menu Status Manage Selected Card Selector
            //if (treeViewDiscardPileHand.SelectedImageIndex == 0 || treeViewPlayer1Hand.SelectedImageIndex == 0 || treeViewPlayer2Hand.SelectedImageIndex == 0 || treeViewPlayer3Hand.SelectedImageIndex == 0)
            //{
            //    comboBoxManageSelectedCard.Enabled = true;
            //}
            //else
            //{
            //    comboBoxManageSelectedCard.Enabled = false;
            //}

            //Menu Status Ready To Play 
            if (CameraActive && Players.Count > 1 && !GameLive)// && comboBoxRemotePlayerSelection.SelectedIndex == 0
            {
                buttonRemotePlay.Enabled = true;
                buttonManagePlay.Enabled = true;
                GameTurns = 0;
            }
            else
            {
                buttonRemotePlay.Enabled = false;
                buttonManagePlay.Enabled = false;
            }

            //if (treeViewDiscardPileHand.Leave != null || treeViewPlayer1Hand.SelectedNode != null || treeViewPlayer2Hand.SelectedNode != null || treeViewPlayer3Hand.SelectedNode != null)//If Card on maintanance screen is selected 
            //{
            //    comboBoxManageSelectedCard.Enabled = true;
            //}
            //else
            //{
            //    comboBoxManageSelectedCard.Enabled = false;
            //}


            //Menu Status Game Playing
            if (GameLive && GameRunning)
            {
                buttonRemotePause.Enabled = true;
                buttonManagePause.Enabled = true;
                comboBoxRemotePlayerSelection.Enabled = false;
                checkBoxA550C_1.Enabled = false;
                checkBoxA550_2.Enabled = false;
                checkBoxFC006N_3.Enabled = false;
                checkBoxHUMAN.Enabled = false;
                numericUpDownStartCardAmount.Enabled = false;

                labelGameDirection.Text = GameDirectionClockwise ? "Direction: CW" : "Direction CCW";
                labelTurnNumber.Text = "Turn#: " + GameTurns.ToString();
                labelPlayersTurn.Text = "Turn: " + PlayerData[PlayersTurn][0];

                //Remote Player Can Taunt
                if (PlayerCanTaunt)
                {
                    buttonPlayTauntLeft.Enabled = true;
                    buttonPlayTauntRight.Enabled = true;
                }
                else
                {
                    buttonPlayTauntLeft.Enabled = false;
                    buttonPlayTauntRight.Enabled = false;
                }

                if (!ShuffleCardRunning && !RobotPlayingCard && RemotePlayerSelectedCardIndex != RemotePlayerSelectedCardNullValue)
                {
                    buttonPlayPlayCard.Enabled = true;
                }
                else
                {
                    buttonPlayPlayCard.Enabled = false;
                }
                if ((RemotePlayerDrawingTwoOrFour && !RobotRetrievingCard) || 
                    (!RobotRetrievingCard && PlayerData[PlayersTurn][0] == RemotePlayerName && ((PlayerData[PlayersTurn][1] == null ^ int.Parse(PlayerData[PlayersTurn][1]) == 0) || 
                    (int.Parse(PlayerData[PlayersTurn][1]) == 1 && !FirstCardPlayed) || 
                    (CardPlayabilityFound && !flowLayoutPanelPlayersCards.Controls.OfType<Control>().Any(control => control.Enabled)))))
                {
                    buttonPlayDrawCard.Enabled = true;
                }
                else
                {
                    buttonPlayDrawCard.Enabled = false;
                }
                //set button text to new functionality when Playing First Card
                if (int.Parse(PlayerData[PlayersTurn][1]) == 1 && !FirstCardPlayed)
                {
                    buttonPlayDrawCard.Text = "Play First Card";
                }
                else
                {
                    buttonPlayDrawCard.Text = "Draw Card";
                }

                
               

                //If the Player Is An NPC Robot
                if (PlayerData[PlayersTurn][0] != "Human" && PlayerData[PlayersTurn][0] != RemotePlayerName)
                {
                    //Console.WriteLine("NPCs Turn");
                    pictureBoxDiscardPile.BackColor = Color.Black;

                    //Console.WriteLine("1");
                    //First turn of the game Draw Cards
                    if (int.Parse(PlayerData[PlayersTurn][1]) == 0)
                    {
                        if (!NPCDrawCardSent)
                        {
                            SendDrawSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 1);
                            NPCDrawCardSent = true;
                        }
                        if (DrawCardCompleted)
                        {
                            var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                            DrawnPredictedCard = Card.PredictedCard.ToString();
                            DrawnPredictedCardID = Card.CardID;

                            //Speak Card
                            //RemoteChatSend(Card.PredictedCard, "System");

                            if (Card.PredictedCard.ToString() == "UNO")
                            {
                                DrawnPredictedCard = "Blue 0";
                            }
                            if (Card.CardID == 52)
                            {
                                DrawnPredictedCardID = 0;
                            }
                            PlayerData[PlayersTurn].Add(DrawnPredictedCardID.ToString());
                            UpdatePlayerDataDependants();

                            NPCDrawCardSent = false;
                            DrawCardCompleted = false;
                        }

                        //Add to player turn count if player has 7 cards
                        if ((PlayerData[PlayersTurn].Count - 2) == StartingCardsCount)
                        {
                            Console.WriteLine($"Player at {StartingCardsCount} cards");
                            PlayerData[PlayersTurn].Add(PlayerDataTerminator.ToString());
                            UpdatePlayerDataDependants();
                            PreviousPlayerPlayedCard = false;
                            NPCRobotTurnEndCall = true;
                        }
                    }
                    

                    //All Other Turns except first turn card flip
                    else if (int.Parse(PlayerData[PlayersTurn][1]) > 0 && FirstCardPlayed && TauntCompleted && !NPCRobotTurnEndCall)
                    {
                        //if (!CheckForUno)
                        //{
                        //    bool SayUno = false;
                        //    foreach (List<string> PlayersData in PlayerData)
                        //    {
                        //        if (PlayersData[0] == "Human" && (int.Parse(PlayersData[2]) == 1))
                        //        {
                        //            //If Human Has One Card
                        //            SayUno = true;
                        //            break;
                        //        }
                        //        else if (PlayersData.Count == 4)
                        //        {
                        //            //If Other Player Has One Card
                        //            SayUno = true;
                        //            break;
                        //        }
                        //    }
                        //    if (SayUno)
                        //    {
                        //        //Say Uno
                        //        RemoteChatSend("Uno!", "System");
                        //    }

                        //    CheckForUno = true;
                        //}
                        if (PreviousPlayerPlayedCard)
                        {//if card is new, find new card and respect effects 
                            if (DiscardCardIDFound == false)
                            {
                                var Card = PredictCard(0);
                                DiscardCardID = Card.CardID;

                                int PreviousPlayer = 0;
                                if (GameDirectionClockwise)
                                {
                                    if (PlayersTurn == 0)
                                    {//set to last player
                                        PreviousPlayer = Players.Count - 1;
                                    }
                                    else
                                    {//set to players turn -1 
                                        PreviousPlayer = PlayersTurn - 1;
                                    }
                                }
                                else
                                {
                                    if (PlayersTurn == (Players.Count - 1))
                                    {//set to first player
                                        PreviousPlayer = 0;
                                    }
                                    else
                                    {//set to players turn +1 
                                        PreviousPlayer = PlayersTurn + 1;
                                    }
                                }

                                if (PlayerData[PreviousPlayer][0] != "Human" && (DiscardCardID == 53 || DiscardCardID == 54))
                                {//if previous player was not a human and they played a wild card, do not overide solved color
                                    //do not add card, chosen card color already added 

                                    //Speak Card
                                    RemoteChatSend(GetNotEnthusiasticResponse(treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text), "System");
                                }
                                else
                                {
                                    //add card
                                    treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);

                                    //Speak Card
                                    RemoteChatSend(GetNotEnthusiasticResponse(Card.PredictedCard), "System");
                                }

                                

                                UpdatePlayerDataDependants();
                                PlayerSeeWildCardInitializeVars = false;
                                DiscardCardIDFound = true;
                                //PlayerCanTaunt = true;
                            }

                            if (DiscardCardIDFound && !CardPlayabilityFound)
                            {
                                //Sense and exicute here effects of special cards 
                                if (DiscardCardID >= 40 && !PlayerIgnoreCardConsequenses)
                                {//If card has effects and special card has not spent effects
                                    if (!PlayerSeeWildCardInitializeVars)
                                    {//run Console feedback once to prevent terminal flodding 
                                        Console.WriteLine("special card has not spent effects, executing effects...");
                                        PlayerSeeWildCardInitializeVars = true;
                                    }
                                    if (DiscardCardID == 40 || DiscardCardID == 43 || DiscardCardID == 46 || DiscardCardID == 49)
                                    {//draw 2
                                        if (!PlayerDrawingTwoOrFoInitializeVars)
                                        {
                                            Console.WriteLine("draw 2");
                                            DrawnCards = 0;
                                            PlayerDrawingTwoOrFoInitializeVars = true;
                                        }
                                        if (!NPCDrawCardSent)
                                        {
                                            RemoteChatSend(GetNotEnthusiasticDrawCardResponse(), "System");

                                            SendDrawSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 1);
                                            NPCDrawCardSent = true;
                                        }
                                        if (DrawCardCompleted)
                                        {
                                            var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                            DrawnPredictedCard = Card.PredictedCard.ToString();
                                            DrawnPredictedCardID = Card.CardID;

                                            //Speak Card
                                            //RemoteChatSend(Card.PredictedCard, "System");

                                            if (Card.PredictedCard.ToString() == "UNO")
                                            {
                                                DrawnPredictedCard = "Blue 0";
                                            }
                                            if (Card.CardID == 52)
                                            {
                                                DrawnPredictedCardID = 0;
                                            }
                                            PlayerData[PlayersTurn].Insert(PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString()), DrawnPredictedCardID.ToString());
                                            UpdatePlayerDataDependants();
                                            PrintListOfStringLists(PlayerData);
                                            DrawnCards++;
                                            NPCDrawCardSent = false;
                                            DrawCardCompleted = false;
                                        }

                                        //Add to player turn count if player has 2 cards
                                        if (DrawnCards == 2)
                                        {
                                            Console.WriteLine($"Player drew 2 cards");
                                            //For the next turn ignore special cards efects since they would have already been spent
                                            PlayerDrawingTwoOrFoInitializeVars = false;
                                            PlayerIgnoreCardConsequenses = true;
                                            //end turn
                                            PreviousPlayerPlayedCard = false;
                                            NPCRobotTurnEndCall = true;
                                        }
                                    }
                                    else if (DiscardCardID == 41 || DiscardCardID == 44 || DiscardCardID == 47 || DiscardCardID == 50)
                                    {//reverse
                                        Console.WriteLine("reverse");
                                        PrintListOfStringLists(PlayerData);
                                        UpdatePlayerDataDependants();
                                        //change direction

                                        GameDirectionClockwise = !GameDirectionClockwise;
                                        //if (GameDirectionClockwise)
                                        //{
                                        //    GameDirectionClockwise = false;
                                        //}
                                        //else
                                        //{
                                        //    GameDirectionClockwise = true;
                                        //}

                                        //Skip next player to prevent reverse giving player additional turn
                                        if (Players.Count > 2)
                                        {
                                            //only if there are more than 2 players else make reverse act as skip card in 1v1
                                            SkipNextPlayerOnAdvanceTurn = true;
                                        }

                                        //For the next turn ignore special cards efects since they would have already been spent
                                        PlayerIgnoreCardConsequenses = true;
                                        PreviousPlayerPlayedCard = false;
                                        NPCRobotTurnEndCall = true;
                                    }
                                    else if (DiscardCardID == 42 || DiscardCardID == 45 || DiscardCardID == 48 || DiscardCardID == 51)
                                    {//skip
                                        Console.WriteLine("skip");
                                        PrintListOfStringLists(PlayerData);
                                        UpdatePlayerDataDependants();
                                        //For the next turn ignore special cards efects since they would have already been spent
                                        PlayerIgnoreCardConsequenses = true;
                                        //skip player
                                        PreviousPlayerPlayedCard = false;
                                        NPCRobotTurnEndCall = true;
                                    }
                                    else if (DiscardCardID == 53)
                                    {//Wild
                                     //check if color has already been solved
                                        if (treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text == "Wild")
                                        {//then we need to find out color
                                            if (HumanChooseColorAskState)
                                            {//prompt human for color selection 
                                                Console.WriteLine("Wild");
                                                RemoteChatSend(GetRandomCardColorPrompts(), "System");//whats the color?
                                                HumanChooseColorAskState = false;
                                                HumanChooseColorWaitForResponceState = true;
                                            }
                                            if (HumanChooseColorRepeatColorChosenState)
                                            {
                                                Console.WriteLine("Asked for color verification");
                                                //define string to plug in user responce
                                                ChosenColorVerificationMessages = new string[]
                                                {
                                            $"Did you choose {HumanChooseColor} for your wild card?",
                                            $"Is the color you selected for the wild card {HumanChooseColor}?",
                                            $"Confirm if your wild card color is {HumanChooseColor}.",
                                            $"Is the wild card color you picked {HumanChooseColor}?",
                                            $"Are you confirming {HumanChooseColor} as your wild card color?",
                                            $"Did you decide on {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the color of your chosen wild card?",
                                            $"Confirm that the wild card color you chose is {HumanChooseColor}.",
                                            $"Are you going with the color {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the designated color for your wild card?",
                                            $"Confirm your choice: is it the color {HumanChooseColor} for your wild card?",
                                            $"Did you pick {HumanChooseColor} as the color for your wild card?",
                                            $"Is your wild card color officially {HumanChooseColor}?",
                                            $"Affirm if the color of your wild card is {HumanChooseColor}.",
                                            $"Did you finalize {HumanChooseColor} as your wild card color?",
                                            $"Is {HumanChooseColor} the color you've chosen for your wild card?",
                                            $"Can you confirm that the wild card color is {HumanChooseColor}?",
                                            $"Did you settle on {HumanChooseColor} for your wild card?",
                                            $"Is the color {HumanChooseColor} officially chosen for your wild card?",
                                            $"Confirm that {HumanChooseColor} is the color you've chosen for your wild card."
                                                };

                                                RemoteChatSend(GetRandomChosenColorVerificationMessages(), "System");// the color is "red"?  Exception thrown: 'System.InvalidOperationException' in System.Speech.dll

                                                HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                HumanChooseColorRepeatColorChosenState = false;
                                                HumanChooseColorWaitForConformationStateInitializeVars = false;
                                                HumanChooseColorWaitForConformationState = true;
                                            }
                                            if (HumanChooseColorPromptCorrectResponceState)//HumanChooseColorRepeatColorChosenState
                                            {
                                                RemoteChatSend(GetRandomMisunderstoodColorResponce(), "System");//oops my bad what color is it 

                                                HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                HumanChooseColorWaitForResponceState = true;
                                                HumanChooseColorPromptCorrectResponceState = false;
                                            }
                                            if (HumanChooseColorWaitForConformationState)
                                            {
                                                if (!HumanChooseColorWaitForConformationStateInitializeVars)
                                                {
                                                    Console.WriteLine("Waiting for conformation");
                                                    HumanChooseColorWaitForConformationStateInitializeVars = true;
                                                }

                                                if (Environment.TickCount > HumanChooseColorWaitForConformationStartTime + HumanChooseColorWaitForConformationTime)
                                                {
                                                    HumanChooseColorWaitForConformationState = false;
                                                    HumanChooseColorExicuteResponceState = true;
                                                }
                                            }
                                            if (HumanChooseColorExicuteResponceState)
                                            {
                                                Console.WriteLine("Wild Card Exicuted");
                                                treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text = HumanChooseColor;
                                                DiscardCardID = Array.IndexOf(UNOCards, HumanChooseColor);
                                                UpdatePlayerDataDependants();
                                                CardPlayabilityFound = true;
                                                HumanChooseColorExicuteResponceState = false;
                                            }
                                        }
                                        else
                                        {//Treat wild card after color solve
                                            Console.WriteLine("Wild Card Exicuted");
                                            DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                                            UpdatePlayerDataDependants();
                                            CardPlayabilityFound = true;
                                        }

                                    }
                                    else if (DiscardCardID == 54)
                                    {//Wild draw 4
                                     //Step 1 DRAW 4
                                        if (!PlayerDrawingTwoOrFoInitializeVars)
                                        {
                                            Console.WriteLine("Wild draw 4");
                                            DrawnCards = 0;
                                            PlayerDrawingTwoOrFoInitializeVars = true;
                                        }
                                        if (!NPCDrawCardSent && !RemotePlayerDrawFourAskColorStep)
                                        {
                                            RemoteChatSend(GetNotEnthusiasticDrawCardResponse(), "System");

                                            SendDrawSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 1);
                                            NPCDrawCardSent = true;
                                        }
                                        if (DrawCardCompleted)
                                        {
                                            var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                            DrawnPredictedCard = Card.PredictedCard.ToString();
                                            DrawnPredictedCardID = Card.CardID;

                                            //Speak Card
                                            //RemoteChatSend(Card.PredictedCard, "System");

                                            if (Card.PredictedCard.ToString() == "UNO")
                                            {
                                                DrawnPredictedCard = "Blue 0";
                                            }
                                            if (Card.CardID == 52)
                                            {
                                                DrawnPredictedCardID = 0;
                                            }
                                            PlayerData[PlayersTurn].Insert(PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString()), DrawnPredictedCardID.ToString());
                                            UpdatePlayerDataDependants();
                                            PrintListOfStringLists(PlayerData);
                                            DrawnCards++;
                                            NPCDrawCardSent = false;
                                            DrawCardCompleted = false;
                                        }

                                        //Add to player turn count if player has 4 cards
                                        if (DrawnCards == 4)
                                        {
                                            Console.WriteLine($"Player drew 4 cards");
                                            //For the next turn ignore special cards efects since they would have already been spent
                                            PlayerDrawingTwoOrFoInitializeVars = false;
                                            RemotePlayerDrawFourAskColorStep = true;
                                        }

                                        //Step 2 ASK COLOR 
                                        if (RemotePlayerDrawFourAskColorStep)
                                        {
                                            //check if color has already been solved
                                            if (treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text == "Wild Draw 4")
                                            {//then we need to find out color
                                                if (HumanChooseColorAskState)
                                                {//prompt human for color selection 
                                                    Console.WriteLine("Wild draw 4");
                                                    RemoteChatSend(GetRandomCardColorPrompts(), "System");//whats the color?
                                                    HumanChooseColorAskState = false;
                                                    HumanChooseColorWaitForResponceState = true;
                                                }
                                                if (HumanChooseColorRepeatColorChosenState)
                                                {
                                                    Console.WriteLine("Asked for color verification");
                                                    //define string to plug in user responce
                                                    ChosenColorVerificationMessages = new string[]
                                                    {
                                            $"Did you choose {HumanChooseColor} for your wild card?",
                                            $"Is the color you selected for the wild card {HumanChooseColor}?",
                                            $"Confirm if your wild card color is {HumanChooseColor}.",
                                            $"Is the wild card color you picked {HumanChooseColor}?",
                                            $"Are you confirming {HumanChooseColor} as your wild card color?",
                                            $"Did you decide on {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the color of your chosen wild card?",
                                            $"Confirm that the wild card color you chose is {HumanChooseColor}.",
                                            $"Are you going with the color {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the designated color for your wild card?",
                                            $"Confirm your choice: is it the color {HumanChooseColor} for your wild card?",
                                            $"Did you pick {HumanChooseColor} as the color for your wild card?",
                                            $"Is your wild card color officially {HumanChooseColor}?",
                                            $"Affirm if the color of your wild card is {HumanChooseColor}.",
                                            $"Did you finalize {HumanChooseColor} as your wild card color?",
                                            $"Is {HumanChooseColor} the color you've chosen for your wild card?",
                                            $"Can you confirm that the wild card color is {HumanChooseColor}?",
                                            $"Did you settle on {HumanChooseColor} for your wild card?",
                                            $"Is the color {HumanChooseColor} officially chosen for your wild card?",
                                            $"Confirm that {HumanChooseColor} is the color you've chosen for your wild card."
                                                    };

                                                    RemoteChatSend(GetRandomChosenColorVerificationMessages(), "System");// the color is "red"?  Exception thrown: 'System.InvalidOperationException' in System.Speech.dll

                                                    HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                    HumanChooseColorRepeatColorChosenState = false;
                                                    HumanChooseColorWaitForConformationStateInitializeVars = false;
                                                    HumanChooseColorWaitForConformationState = true;
                                                }
                                                if (HumanChooseColorPromptCorrectResponceState)//HumanChooseColorRepeatColorChosenState
                                                {
                                                    RemoteChatSend(GetRandomMisunderstoodColorResponce(), "System");//oops my bad what color is it 

                                                    HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                    HumanChooseColorWaitForResponceState = true;
                                                    HumanChooseColorPromptCorrectResponceState = false;
                                                }
                                                if (HumanChooseColorWaitForConformationState)
                                                {
                                                    if (!HumanChooseColorWaitForConformationStateInitializeVars)
                                                    {
                                                        Console.WriteLine("Waiting for conformation");
                                                        HumanChooseColorWaitForConformationStateInitializeVars = true;
                                                    }

                                                    if (Environment.TickCount > HumanChooseColorWaitForConformationStartTime + HumanChooseColorWaitForConformationTime)
                                                    {
                                                        HumanChooseColorWaitForConformationState = false;
                                                        HumanChooseColorExicuteResponceState = true;
                                                    }
                                                }
                                                if (HumanChooseColorExicuteResponceState)
                                                {
                                                    Console.WriteLine("Wild draw 4 Card Exicuted");
                                                    treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text = HumanChooseColor;
                                                    DiscardCardID = Array.IndexOf(UNOCards, HumanChooseColor);
                                                    UpdatePlayerDataDependants();
                                                    PlayerIgnoreCardConsequenses = true;
                                                    PreviousPlayerPlayedCard = false;
                                                    NPCRobotTurnEndCall = true;
                                                    HumanChooseColorExicuteResponceState = false;
                                                }
                                            }
                                            else
                                            {//Treat wild card after color solve
                                                Console.WriteLine("Wild draw 4 Card Exicuted");
                                                DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                                                UpdatePlayerDataDependants();
                                                PlayerIgnoreCardConsequenses = true;
                                                PreviousPlayerPlayedCard = false;
                                                NPCRobotTurnEndCall = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {//all other cards
                                    UpdatePlayerDataDependants();

                                    CardPlayabilityFound = true;
                                }
                            }
                        }
                        else //(!PreviousPlayerPlayedCard)
                        {//card is the same as last turn, Find card id unless last tree view is a solved wild, then dont overide solved color
                            if (!DiscardCardIDFound)
                            {
                                Console.WriteLine("card is the same as last turn, Find card id unless last tree view is a solved wild, then dont overide solved color");
                                var Card = PredictCard(0);
                                DiscardCardID = Card.CardID;
                                
                                //Update Dicard Prediction Preview
                                Console.WriteLine($"Last Discard Card = {treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text}");
                                if ((treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Blue"
                                    && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Green"
                                    && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Red"
                                    && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Yellow"))
                                {//if the card is not a solved wild card then add the found card 
                                    Console.WriteLine($"the card is not a solved wild card, add the found card: {UNOCards[DiscardCardID]}");
                                    treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);

                                    //Speak Card
                                    RemoteChatSend(GetNotEnthusiasticResponse(Card.PredictedCard), "System");
                                }
                                else
                                {//if the card is a solved wild card, keep the color found
                                    Console.WriteLine($"the card({UNOCards[DiscardCardID]}) is a solved wild card, keep the color found: {treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text}");
                                    //treeViewDiscardPileHand.Nodes.Add(treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1]);
                                    DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);

                                    //Speak Card
                                    RemoteChatSend(GetNotEnthusiasticResponse(treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text), "System");
                                }
                                //PlayerCanTaunt = true;
                                UpdatePlayerDataDependants();
                                DiscardCardIDFound = true;
                                CardPlayabilityFound = true;
                            }
                        }

                        if (CardPlayabilityFound)
                        {

                            if (!CardPlayabilityFoundInitializeVars)
                            {//update players hand
                                UpdatePlayerDataDependants();
                                RemoteChatSend("My Turn", "System");
                                //For the next turn ignore special cards efects since they would have already been spent
                                PlayerIgnoreCardConsequenses = true;
                                CardPlayabilityFoundInitializeVars = true;
                                NPCRobotDrewCardAlready = false;
                            }
                            
                            //if no cards are playable than draw
                            if (NPCCheckCardPlayability)
                            {
                                List<int> PlayableCardsIndexes = new List<int>();
                                //PlayableCardsIndexes.add(0);
                                //PlayableCardsIndexes.Clear();

                                Console.WriteLine($"Checking Player {PlayerData[PlayersTurn][0]} Cards");

                                for (int i = 2; i < PlayerData[PlayersTurn].Count; i++)
                                {//go through every card in players hand
                                    Console.WriteLine($"Checking Card {PlayerData[PlayersTurn][i]} is compatable with card {DiscardCardID}");

                                    if (CardVCard[int.Parse(PlayerData[PlayersTurn][i])].Contains(DiscardCardID))
                                    {//if card is playable to the discard, count
                                        PlayableCardsIndexes.add(i);

                                        Console.WriteLine($"Card {PlayerData[PlayersTurn][i]} is compatable with card {DiscardCardID}");
                                    }
                                }
                                NPCCheckCardPlayability = false;//only run once

                                //RemoteChatSend($"{PlayableCardsIndexes.Count} Cards Playable", "System");

                                if (PlayableCardsIndexes.Count > 0)
                                {//if there are one or more playable cards, play one
                                    //RemoteChatSend("Playing Card", "System");
                                    if (NPCRobotDrewCardAlready)
                                    {
                                        //If robot can play card on second try
                                        RemoteChatSend(GetDrewCardCanPlayResponse(), "System");

                                    }

                                    int CardPlayingTerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());
                                    //Find what card id to play and its index

                                    Console.WriteLine($"FindClosestIndex = {FindClosestIndex(PlayableCardsIndexes, CardPlayingTerminatorIndex)}");

                                    PlayerCardPlayingID = int.Parse(PlayerData[PlayersTurn][FindClosestIndex(PlayableCardsIndexes, CardPlayingTerminatorIndex)]);
                                    PlayerCardPlayingIndex = FindClosestIndex(PlayableCardsIndexes, CardPlayingTerminatorIndex);



                                    if (PlayerCardPlayingID == 53 || PlayerCardPlayingID == 54)
                                    {//If player is playing a wild card thet they need to select color of
                                        Console.WriteLine($"Player Needs to chose Color of wild card");

                                        //Choose the color of wild card
                                        //make a list of all the card names
                                        List<string> PlayersCardNames = new List<string>();
                                        for (int i = 2; i < PlayerData[PlayersTurn].Count; i++)
                                        {
                                            //add the name of every card to list
                                            PlayersCardNames.add(UNOCards[int.Parse(PlayerData[PlayersTurn][i])]);
                                        }

                                        //find most common color of card
                                        NPCWildCardColorChoice = FindMostFrequentColor(PlayersCardNames);

                                        if (NPCWildCardColorChoice == "Blue")
                                        {
                                            treeViewDiscardPileHand.Nodes.Add("Blue");

                                        }
                                        else if (NPCWildCardColorChoice == "Green")
                                        {
                                            treeViewDiscardPileHand.Nodes.Add("Green");

                                        }
                                        else if (NPCWildCardColorChoice == "Red")
                                        {
                                            treeViewDiscardPileHand.Nodes.Add("Red");

                                        }
                                        else if (NPCWildCardColorChoice == "Yellow")
                                        {
                                            treeViewDiscardPileHand.Nodes.Add("Yellow");

                                        }

                                        PlayedCardColorChosen = true;
                                    }

                                    

                                    int TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());

                                        if (PlayerCardPlayingIndex < TerminatorIndex)
                                        {
                                            //Left Pile
                                            Console.WriteLine($"Player Data:");
                                            PrintListOfStringLists(PlayerData);
                                            Console.WriteLine($"Playing Card In Left Pile. RemotePlayerSelectedCardIndex=({PlayerCardPlayingIndex}), TerminatorIndex=({TerminatorIndex})");
                                            SendShuffleSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 2, Math.Abs((PlayerCardPlayingIndex) - TerminatorIndex));
                                            Console.WriteLine($"Coil {((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 2} run for {Math.Abs((PlayerCardPlayingIndex) - TerminatorIndex)} Iterations whith abs of: {PlayerCardPlayingIndex + 2} - {TerminatorIndex} for the left pile");
                                            
                                        }
                                        else if (PlayerCardPlayingIndex >= TerminatorIndex)
                                        {
                                            //Right Pile
                                            Console.WriteLine($"Player Data:");
                                            PrintListOfStringLists(PlayerData);
                                            Console.WriteLine($"Card In Right Pile. RemotePlayerSelectedCardIndex=({PlayerCardPlayingIndex}), TerminatorIndex=({TerminatorIndex})");
                                            SendShuffleSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 3, Math.Abs((PlayerCardPlayingIndex) - TerminatorIndex));
                                            Console.WriteLine($"Coil {((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 3} run for {Math.Abs((PlayerCardPlayingIndex + 1) - TerminatorIndex)} Iterations whith abs of: {RemotePlayerSelectedCardIndex + 3} - {TerminatorIndex} for the right pile");
                                        }

                                }
                                else
                                {//else if there are no playable cards, draw one if one hasent been drawn already
                                    if (NPCRobotDrewCardAlready)
                                    {
                                        //RemoteChatSend($"Turn Over", "System");
                                        PreviousPlayerPlayedCard = false;
                                        PlayerIgnoreCardConsequenses = true;

                                        //end turn
                                        NPCRobotTurnEndCall = true;
                                    }
                                    else
                                    {
                                        //Draw Card
                                        SendDrawSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 1);

                                        RemoteChatSend(GetNotEnthusiasticDrawCardResponse(), "System");

                                        //mark that the player has drawn card
                                        NPCRobotDrewCardAlready = true;
                                    }
                                }
                            }

                            if (ShuffleCardCompleted)//RemoteTurnOver
                            {
                                //Console.WriteLine($"Remove {Array.IndexOf(UNOCards, PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 2])} at {RemotePlayerSelectedCardIndex + 2}");
                                //PlayerData[PlayersTurn].RemoveAt(RemotePlayerSelectedCardIndex + 2);
                                PrintListOfStringLists(PlayerData);
                                Console.WriteLine($"{PlayerCardPlayingIndex} is before the index of {PlayerDataTerminator} ({PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())}.");
                                PreviousPlayerPlayedCard = true;
                                PlayerIgnoreCardConsequenses = false;
                                NPCRobotTurnEndCall = true;
                                RobotPlayingCard = false;
                            }

                            if (DrawCardCompleted)
                            {
                                var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                DrawnPredictedCard = Card.PredictedCard.ToString();
                                DrawnPredictedCardID = Card.CardID;

                                //Speak Card
                                //RemoteChatSend(Card.PredictedCard, "System");

                                if (Card.PredictedCard.ToString() == "UNO")
                                {
                                    DrawnPredictedCard = "Blue 0";
                                }
                                if (Card.CardID == 52)
                                {
                                    DrawnPredictedCardID = 0;
                                }
                                PlayerData[PlayersTurn].Insert(PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString()), DrawnPredictedCardID.ToString());
                                PrintListOfStringLists(PlayerData);
                                //recall to check card playability 
                                NPCCheckCardPlayability = true;

                                DrawCardCompleted = false;
                            }
                        }
                    }

                    //Second turn of the game, flip first card?  FirstCardPlayed
                    if (int.Parse(PlayerData[PlayersTurn][1]) == 1 && !FirstCardPlayed)
                    {
                        if (!NPCPlayingFirstCardInitializeVars)
                        {
                            Console.WriteLine("NPC, Playing First Card");
                            SendDrawSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 1);

                            NPCPlayingFirstCardInitializeVars = true;
                        }

                        if (DrawCardCompleted)
                        {//once player draws card, play the card
                            if (!NPCShuffelSigSent)
                            {//imediatly place card after draw
                                Console.WriteLine("Remote Player drew card Card, placing card...");
                                SendShuffleSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 2, 1, false);
                                NPCShuffelSigSent = true;
                            }
                            if (ShuffleCardCompleted)
                            {//advance turn
                                int FirstDrawnPredictedCardID = PredictCard(0).CardID;
                                //If first card is wild then draw again
                                if (FirstDrawnPredictedCardID == 53 || FirstDrawnPredictedCardID == 54)
                                {
                                    NPCPlayingFirstCardInitializeVars = false;
                                    NPCShuffelSigSent = false;
                                }
                                //If first card is not wild then end turn
                                else
                                {
                                    NPCPlayingFirstCardInitializeVars = false;
                                    PreviousPlayerPlayedCard = true;
                                    NPCRobotTurnEndCall = true;
                                    FirstCardPlayed = true;
                                }
                            }
                        }
                    }

                    if (NPCRobotTurnEndCall)
                    {
                        if (!NPCTauntSugnalSent)
                        {
                            CheckForUno = false;
                            NPCTauntSugnalSent = true;
                            PlayerCanTaunt = false;
                            DrawCardCompleted = false;
                            NPCShuffelSigSent = false;
                            ShuffleCardCompleted = false;
                            DiscardCardIDFound = false;
                            CardPlayabilityFound = false;
                            RemotePlayerDrawFourAskColorStep = false;
                            HumanChooseColorAskState = true;
                            NPCCheckCardPlayability = true;
                            NPCRobotDrewCardAlready = false;
                            PlayerPlayedCardIDFound = false;
                            PlayedCardColorChosen = false;
                            CardPlayabilityFoundInitializeVars = false;

                            //check if it's robots last card then say Uno
                            if (PlayerData[PlayersTurn].Count == 4)
                            {
                                //player has one card left
                                //Say Uno
                                RemoteChatSend("Uno!", "System");
                            }

                            Console.WriteLine("Check if Game Won");
                            Console.WriteLine("Player Data: ");
                            PrintListOfStringLists(PlayerData);
                            Console.WriteLine($"int.Parse(PlayerData[PlayersTurn][1]) = {int.Parse(PlayerData[PlayersTurn][1])} > 0? = {int.Parse(PlayerData[PlayersTurn][1]) > 0}");
                            Console.WriteLine($"FirstCardPlayed = {FirstCardPlayed} == true? = {FirstCardPlayed}");
                            Console.WriteLine($"PlayerData[PlayersTurn].Count = {PlayerData[PlayersTurn].Count} <= 3? = {PlayerData[PlayersTurn].Count <= 3}");
                            if (int.Parse(PlayerData[PlayersTurn][1]) > 0 && FirstCardPlayed && PlayerData[PlayersTurn].Count <= 3)
                            {//Check if game won
                                Console.WriteLine($"Player {PlayersTurn} won!");
                                Console.WriteLine("Player Data: ");
                                PrintListOfStringLists(PlayerData);
                                Console.WriteLine($"Turn Count: {PlayerData[PlayersTurn].Count}");

                                //Talk Trash
                                RemoteChatSend(GetRobotWonResponse(), "System");

                                SendTauntSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 4);
                                SendTauntSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 5);
                                GameLive = false;

                                return;
                            }

                            if (PlayerCardPlayingID >= 40)
                            {//NPC Playing Special Card
                             //Speek Taunt 
                                RemoteChatSend(GetNotEnthusiasticPlaySpecialCardResponse(), "System");

                                //Taunt
                                int randomIndex = random.Next(1, 2);
                                switch (randomIndex)
                                {
                                    case 1:
                                        Console.WriteLine("Taunt.");
                                        //Send Taunt Signal Based on game direction
                                        if (GameDirectionClockwise)
                                        {
                                            //Taunt Left
                                            SendTauntSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 4);

                                        }
                                        else
                                        {
                                            //Taunt Right
                                            SendTauntSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 5);

                                        }
                                        break;

                                    case 2:
                                        Console.WriteLine("Do Not Taunt.");
                                        TauntCompleted = true;
                                        Console.WriteLine($"TauntCompleted == {TauntCompleted}");
                                        break;

                                    default:
                                        Console.WriteLine("No statement executed.");
                                        TauntCompleted = true;
                                        Console.WriteLine($"TauntCompleted == {TauntCompleted}");
                                        break;
                                }

                                if (PlayerCardPlayingID != 53 && PlayerCardPlayingID != 54)
                                {
                                    PlayerCardPlayingID = 0;
                                }
                            }
                            else
                            {
                                //no special cards playing, do not taunt
                                TauntCompleted = true;
                                Console.WriteLine($"TauntCompleted == {TauntCompleted}");
                            }
                            if (PlayerCardPlayingID == 53 || PlayerCardPlayingID == 54)
                            {//if NPC played wild, say color after card played
                             //Speek color choice
                                RemoteChatSend(GenerateRobotColorChoiceMessage(NPCWildCardColorChoice), "System");
                                PlayerCardPlayingID = 0;
                            }
                        }

                        if (TauntCompleted)
                        {
                            Console.WriteLine("Taunt Completed, Finishing Turn");
                            

                            Console.WriteLine($"PreviousPlayerPlayedCard = {PreviousPlayerPlayedCard}");
                            Console.WriteLine($"TauntCompleted == {TauntCompleted}");

                            RobotRetrievingCard = false;
                            NPCTauntSugnalSent = false;

                            PlayerData[PlayersTurn][1] = (int.Parse(PlayerData[PlayersTurn][1]) + 1).ToString();
                            PrintListOfStringLists(PlayerData);
                            AdvancePlayerTurn();

                            NPCRobotTurnEndCall = false;
                        }
                        
                    }

                }

                //If the Player Is A Remote Player
                if (PlayerData[PlayersTurn][0] == RemotePlayerName)
                {
                    pictureBoxDiscardPile.BackColor = Color.Yellow;
                    //Console.WriteLine("2");
                    //Console.WriteLine($"PlayerData.Count = {PlayerData.Count}, PlayersTurn = {PlayersTurn}");
                    //Console.WriteLine($"PlayerData[PlayersTurn][1] = {PlayerData[PlayersTurn][1]}");

                    //First turn of the game Draw Cards
                    if (PlayerData[PlayersTurn][1] == null || int.Parse(PlayerData[PlayersTurn][1]) == 0)
                    {
                        //draw 7 cards
                        if (DrawButtonEnable)
                        {
                            buttonPlayDrawCard.Enabled = true;
                            DrawnPredictedCardID = 0;
                            DrawnPredictedCard = "";
                        }
                        if (DrawCardCompleted)
                        {
                            
                            if (!CardPredicted)//Only Run this once
                            {
                                labelLoadingPlayerHandPredict.Visible = true;
                                labelLoadingPlayerHandPredict.Update();
                                var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                DrawnPredictedCard = Card.PredictedCard.ToString();
                                DrawnPredictedCardID = Card.CardID;
                                if (Card.PredictedCard.ToString() == "UNO")
                                {
                                    DrawnPredictedCard = "Blue 0";
                                }
                                if (Card.CardID == 52)
                                {
                                    DrawnPredictedCardID = 0;
                                }
                                if (DrawnPredictedCard != "")
                                {
                                    progressBarRemoteCardVerification.Value = 0;
                                    CardPredicted = true;
                                    Console.WriteLine($"CardPredicted {DrawnPredictedCard}");
                                }
                            }
                            if (CardPredicted && comboBoxRemoteManualCardEntry.Visible == false)
                            {
                                labelLoadingPlayerHandPredict.Visible = false;
                                buttonRemoteCardVerifyYES.Visible = true;
                                buttonRemoteCardVerifyNO.Visible = true;
                                progressBarRemoteCardVerification.Visible = true;
                                //RemoteChatSend("This is a " + DrawnPredictedCard + "?", "Card Detection");
                                pictureBoxPlayerHandPredict.Image = Image.FromFile(CardIconFilePath + @"\" + DrawnPredictedCard + CardIconExtention);

                                if (TimerProgressBarTicked)
                                {
                                    if (progressBarRemoteCardVerification.Value >= progressBarRemoteCardVerification.Maximum)
                                    {
                                        progressBarRemoteCardVerification.Value = 0;
                                        TimerCardVerificationPGElapsed = true;
                                    }
                                    else
                                    {
                                        progressBarRemoteCardVerification.Invoke(new Action(() =>
                                        {
                                            progressBarRemoteCardVerification.Value += 1;
                                            progressBarRemoteCardVerification.Update();
                                        }));
                                    }
                                    TimerProgressBarTicked = false;
                                }
                            }
                            
                            if (TimerCardVerificationPGElapsed)//If Card Approval sesson ended
                            {
                                Console.WriteLine($"PlayersTurn: {PlayersTurn}");
                                if (!comboBoxRemoteManualCardEntry.Visible)//If there was no overide
                                {
                                    PlayerData[PlayersTurn].Add(DrawnPredictedCardID.ToString());
                                    PrintListOfStringLists(PlayerData);
                                }
                                pictureBoxPlayerHandPredict.Image = null;
                                buttonRemoteCardVerifyYES.Visible = false;
                                buttonRemoteCardVerifyNO.Visible = false;
                                progressBarRemoteCardVerification.Visible = false;
                                comboBoxRemoteManualCardEntry.Visible = false;
                                buttonRemoteCardCorrectionEnter.Visible = false;
                                CardPredicted = false;
                                DrawCardCompleted = false;
                                DrawButtonEnable = true;
                                TimerCardVerificationPGElapsed = false;
                                RobotRetrievingCard = false;
                                UpdatePlayerDataDependants();
                            }
                        }

                        //Console.WriteLine($"PlayerData[PlayersTurn].Count - 2 = {PlayerData[PlayersTurn].Count - 2}");
                        //Add to player turn count if player has 7 cards
                        if ((PlayerData[PlayersTurn].Count - 2) == StartingCardsCount)
                        {
                            Console.WriteLine($"Player at {StartingCardsCount} cards");
                            PlayerData[PlayersTurn].Add(PlayerDataTerminator.ToString());
                            PreviousPlayerPlayedCard = false;
                            RemotePlayerTurnEndCall = true;
                        }
                    }
                    


                    //All Other Turns except first card flip
                    else if (int.Parse(PlayerData[PlayersTurn][1]) > 0 && FirstCardPlayed)
                    {
                        if (PreviousPlayerPlayedCard)
                        {//if card is new, find new card and respect effects 
                            if (DiscardCardIDFound == false)
                            {
                                labelLoadingPlayerHandPredict.Visible = true;
                                labelLoadingPlayerHandPredict.Update();
                                var Card = PredictCard(0);
                                DiscardCardID = Card.CardID;

                                int PreviousPlayer = 0;
                                if (GameDirectionClockwise)
                                {
                                    if (PlayersTurn == 0)
                                    {//set to last player
                                        PreviousPlayer = Players.Count - 1;
                                    }
                                    else
                                    {//set to players turn -1 
                                        PreviousPlayer = PlayersTurn - 1;
                                    }
                                }
                                else
                                {
                                    if (PlayersTurn == (Players.Count - 1))
                                    {//set to first player
                                        PreviousPlayer = 0;
                                    }
                                    else
                                    {//set to players turn +1 
                                        PreviousPlayer = PlayersTurn + 1;
                                    }
                                }

                                if (PlayerData[PreviousPlayer][0] != "Human" && (DiscardCardID == 53 || DiscardCardID == 54))
                                {//if previous player was not a human and they played a wild card, do not overide solved color
                                    //do not add card, chosen card color already added 
                                }
                                else
                                {
                                    //add card
                                    treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);
                                }

                                //Update Dicard Prediction Preview
                                string imagePath = CardIconFilePath + @"\" + Card.PredictedCard + CardIconExtention;
                                pictureBoxDiscardPrediction.Image = Image.FromFile(imagePath);
                                //treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);
                                RemotePlayerSelectedCardIndex = RemotePlayerSelectedCardNullValue;
                                UpdatePlayerDataDependants();
                                PlayerSeeWildCardInitializeVars = false;
                                DiscardCardIDFound = true;
                                PlayerCanTaunt = true;
                            }

                            if (DiscardCardIDFound && !CardPlayabilityFound)
                            {
                                labelLoadingPlayerHandPredict.Visible = false;
                                //Sense and exicute here effects of special cards 
                                if (DiscardCardID >= 40 && !PlayerIgnoreCardConsequenses)
                                {//If card has effects and special card has not spent effects
                                    if (!PlayerSeeWildCardInitializeVars)
                                    {//run Console feedback once to prevent terminal flodding 
                                        Console.WriteLine("special card has not spent effects, executing effects...");
                                        PlayerSeeWildCardInitializeVars = true;
                                    }
                                    if (DiscardCardID == 40 || DiscardCardID == 43 || DiscardCardID == 46 || DiscardCardID == 49)
                                    {//draw 2
                                        RemotePlayerDrawingTwoOrFour = true;
                                        if (!PlayerDrawingTwoOrFoInitializeVars)
                                        {
                                            Console.WriteLine("draw 2");
                                            DrawnCards = 0;
                                            PlayerDrawingTwoOrFoInitializeVars = true;
                                        }
                                        //draw 2 cards
                                        if (DrawButtonEnable)
                                        {
                                            buttonPlayDrawCard.Enabled = true;
                                            DrawnPredictedCardID = 0;
                                            DrawnPredictedCard = "";
                                        }
                                        if (DrawCardCompleted)
                                        {
                                            if (!CardPredicted)//Only Run this once
                                            {
                                                labelLoadingPlayerHandPredict.Visible = true;
                                                labelLoadingPlayerHandPredict.Update();
                                                var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                                DrawnPredictedCard = Card.PredictedCard.ToString();
                                                DrawnPredictedCardID = Card.CardID;
                                                if (Card.PredictedCard.ToString() == "UNO")
                                                {
                                                    DrawnPredictedCard = "Blue 0";
                                                }
                                                if (Card.CardID == 52)
                                                {
                                                    DrawnPredictedCardID = 0;
                                                }
                                                if (DrawnPredictedCard != "")
                                                {
                                                    progressBarRemoteCardVerification.Value = 0;
                                                    CardPredicted = true;
                                                    Console.WriteLine($"CardPredicted {DrawnPredictedCard}");
                                                }
                                            }
                                            if (CardPredicted && comboBoxRemoteManualCardEntry.Visible == false)
                                            {
                                                labelLoadingPlayerHandPredict.Visible = false;
                                                buttonRemoteCardVerifyYES.Visible = true;
                                                buttonRemoteCardVerifyNO.Visible = true;
                                                progressBarRemoteCardVerification.Visible = true;
                                                //RemoteChatSend("This is a " + DrawnPredictedCard + "?", "Card Detection");
                                                pictureBoxPlayerHandPredict.Image = Image.FromFile(CardIconFilePath + @"\" + DrawnPredictedCard + CardIconExtention);

                                                if (TimerProgressBarTicked)
                                                {
                                                    if (progressBarRemoteCardVerification.Value >= progressBarRemoteCardVerification.Maximum)
                                                    {
                                                        progressBarRemoteCardVerification.Value = 0;
                                                        TimerCardVerificationPGElapsed = true;
                                                    }
                                                    else
                                                    {
                                                        progressBarRemoteCardVerification.Invoke(new Action(() =>
                                                        {
                                                            progressBarRemoteCardVerification.Value += 1;
                                                            progressBarRemoteCardVerification.Update();
                                                        }));
                                                    }
                                                    TimerProgressBarTicked = false;
                                                }
                                            }

                                            if (TimerCardVerificationPGElapsed)//If Card Approval sesson ended
                                            {
                                                DrawnCards++;
                                                Console.WriteLine($"PlayersTurn: {PlayersTurn}");
                                                if (!comboBoxRemoteManualCardEntry.Visible)//If there was no overide
                                                {
                                                    Console.WriteLine($"PlayersTurn: {PlayersTurn} at {PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())}");

                                                    PlayerData[PlayersTurn].Insert(PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString()), DrawnPredictedCardID.ToString());
                                                    PrintListOfStringLists(PlayerData);
                                                }
                                                pictureBoxPlayerHandPredict.Image = null;
                                                buttonRemoteCardVerifyYES.Visible = false;
                                                buttonRemoteCardVerifyNO.Visible = false;
                                                progressBarRemoteCardVerification.Visible = false;
                                                comboBoxRemoteManualCardEntry.Visible = false;
                                                buttonRemoteCardCorrectionEnter.Visible = false;
                                                CardPredicted = false;
                                                DrawCardCompleted = false;
                                                DrawButtonEnable = true;
                                                TimerCardVerificationPGElapsed = false;
                                                RobotRetrievingCard = false;
                                                UpdatePlayerDataDependants();
                                            }
                                        }

                                        //Add to player turn count if player has 2 cards
                                        if (DrawnCards == 2)
                                        {
                                            Console.WriteLine($"Player drew 2 cards");
                                            //For the next turn ignore special cards efects since they would have already been spent
                                            PlayerDrawingTwoOrFoInitializeVars = false;
                                            PlayerIgnoreCardConsequenses = true;
                                            //end turn
                                            PreviousPlayerPlayedCard = false;
                                            RemotePlayerTurnEndCall = true;
                                        }
                                    }
                                    else if (DiscardCardID == 41 || DiscardCardID == 44 || DiscardCardID == 47 || DiscardCardID == 50)
                                    {//reverse
                                        Console.WriteLine("reverse");
                                        PrintListOfStringLists(PlayerData);
                                        UpdatePlayerDataDependants();
                                        //change direction
                                        GameDirectionClockwise = !GameDirectionClockwise;

                                        //if (GameDirectionClockwise)
                                        //{
                                        //    GameDirectionClockwise = false;
                                        //}
                                        //else
                                        //{
                                        //    GameDirectionClockwise = true;
                                        //}

                                        //Skip next player to prevent reverse giving player additional turn
                                        //Skip next player to prevent reverse giving player additional turn
                                        if (Players.Count > 2)
                                        {
                                            //only if there are more than 2 players else make reverse act as skip card in 1v1
                                            SkipNextPlayerOnAdvanceTurn = true;
                                        }

                                        //For the next turn ignore special cards efects since they would have already been spent
                                        PlayerIgnoreCardConsequenses = true;
                                        PreviousPlayerPlayedCard = false;
                                        RemotePlayerTurnEndCall = true;
                                    }
                                    else if (DiscardCardID == 42 || DiscardCardID == 45 || DiscardCardID == 48 || DiscardCardID == 51)
                                    {//skip
                                        Console.WriteLine("skip");
                                        PrintListOfStringLists(PlayerData);
                                        UpdatePlayerDataDependants();
                                        //For the next turn ignore special cards efects since they would have already been spent
                                        PlayerIgnoreCardConsequenses = true;
                                        //skip player
                                        PreviousPlayerPlayedCard = false;
                                        RemotePlayerTurnEndCall = true;
                                    }
                                    else if (DiscardCardID == 53)
                                    {//Wild
                                     //check if color has already been solved
                                        if (treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text == "Wild")
                                        {//then we need to find out color
                                            if (HumanChooseColorAskState)
                                            {//prompt human for color selection 
                                                Console.WriteLine("Wild");
                                                RemoteChatSend(GetRandomCardColorPrompts(), "System");//whats the color?
                                                HumanChooseColorAskState = false;
                                                HumanChooseColorWaitForResponceState = true;
                                            }
                                            if (HumanChooseColorRepeatColorChosenState)
                                            {
                                                Console.WriteLine("Asked for color verification");
                                                //define string to plug in user responce
                                                ChosenColorVerificationMessages = new string[]
                                                {
                                            $"Did you choose {HumanChooseColor} for your wild card?",
                                            $"Is the color you selected for the wild card {HumanChooseColor}?",
                                            $"Confirm if your wild card color is {HumanChooseColor}.",
                                            $"Is the wild card color you picked {HumanChooseColor}?",
                                            $"Are you confirming {HumanChooseColor} as your wild card color?",
                                            $"Did you decide on {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the color of your chosen wild card?",
                                            $"Confirm that the wild card color you chose is {HumanChooseColor}.",
                                            $"Are you going with the color {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the designated color for your wild card?",
                                            $"Confirm your choice: is it the color {HumanChooseColor} for your wild card?",
                                            $"Did you pick {HumanChooseColor} as the color for your wild card?",
                                            $"Is your wild card color officially {HumanChooseColor}?",
                                            $"Affirm if the color of your wild card is {HumanChooseColor}.",
                                            $"Did you finalize {HumanChooseColor} as your wild card color?",
                                            $"Is {HumanChooseColor} the color you've chosen for your wild card?",
                                            $"Can you confirm that the wild card color is {HumanChooseColor}?",
                                            $"Did you settle on {HumanChooseColor} for your wild card?",
                                            $"Is the color {HumanChooseColor} officially chosen for your wild card?",
                                            $"Confirm that {HumanChooseColor} is the color you've chosen for your wild card."
                                                };

                                                RemoteChatSend(GetRandomChosenColorVerificationMessages(), "System");// the color is "red"?  Exception thrown: 'System.InvalidOperationException' in System.Speech.dll

                                                HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                HumanChooseColorRepeatColorChosenState = false;
                                                HumanChooseColorWaitForConformationStateInitializeVars = false;
                                                HumanChooseColorWaitForConformationState = true;
                                            }
                                            if (HumanChooseColorPromptCorrectResponceState)//HumanChooseColorRepeatColorChosenState
                                            {
                                                RemoteChatSend(GetRandomMisunderstoodColorResponce(), "System");//oops my bad what color is it 

                                                HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                HumanChooseColorWaitForResponceState = true;
                                                HumanChooseColorPromptCorrectResponceState = false;
                                            }
                                            if (HumanChooseColorWaitForConformationState)
                                            {
                                                if (!HumanChooseColorWaitForConformationStateInitializeVars)
                                                {
                                                    Console.WriteLine("Waiting for conformation");
                                                    HumanChooseColorWaitForConformationStateInitializeVars = true;
                                                }

                                                if (Environment.TickCount > HumanChooseColorWaitForConformationStartTime + HumanChooseColorWaitForConformationTime)
                                                {
                                                    HumanChooseColorWaitForConformationState = false;
                                                    HumanChooseColorExicuteResponceState = true;
                                                }
                                            }
                                            if (HumanChooseColorExicuteResponceState)
                                            {
                                                Console.WriteLine("Wild Card Exicuted");
                                                treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text = HumanChooseColor;
                                                DiscardCardID = Array.IndexOf(UNOCards, HumanChooseColor);
                                                UpdatePlayerDataDependants(true);
                                                CardPlayabilityFound = true;
                                                HumanChooseColorExicuteResponceState = false;
                                            }
                                        }
                                        else
                                        {//Treat wild card after color solve
                                            Console.WriteLine("Wild Card Exicuted");
                                            DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                                            UpdatePlayerDataDependants(true);
                                            CardPlayabilityFound = true;
                                        }

                                    }
                                    else if (DiscardCardID == 54)
                                    {//Wild draw 4
                                     //Step 1 DRAW 4
                                        RemotePlayerDrawingTwoOrFour = true;
                                        if (!PlayerDrawingTwoOrFoInitializeVars)
                                        {
                                            Console.WriteLine("Wild draw 4");
                                            DrawnCards = 0;
                                            PlayerDrawingTwoOrFoInitializeVars = true;
                                        }
                                        //draw 4 cards
                                        if (DrawButtonEnable)
                                        {
                                            buttonPlayDrawCard.Enabled = true;
                                            DrawnPredictedCardID = 0;
                                            DrawnPredictedCard = "";
                                        }
                                        if (DrawCardCompleted)
                                        {
                                            if (!CardPredicted)//Only Run this once
                                            {
                                                labelLoadingPlayerHandPredict.Visible = true;
                                                labelLoadingPlayerHandPredict.Update();
                                                var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                                DrawnPredictedCard = Card.PredictedCard.ToString();
                                                DrawnPredictedCardID = Card.CardID;
                                                if (Card.PredictedCard.ToString() == "UNO")
                                                {
                                                    DrawnPredictedCard = "Blue 0";
                                                }
                                                if (Card.CardID == 52)
                                                {
                                                    DrawnPredictedCardID = 0;
                                                }
                                                if (DrawnPredictedCard != "")
                                                {
                                                    progressBarRemoteCardVerification.Value = 0;
                                                    CardPredicted = true;
                                                    Console.WriteLine($"CardPredicted {DrawnPredictedCard}");
                                                }
                                            }
                                            if (CardPredicted && comboBoxRemoteManualCardEntry.Visible == false)
                                            {
                                                labelLoadingPlayerHandPredict.Visible = false;
                                                buttonRemoteCardVerifyYES.Visible = true;
                                                buttonRemoteCardVerifyNO.Visible = true;
                                                progressBarRemoteCardVerification.Visible = true;
                                                //RemoteChatSend("This is a " + DrawnPredictedCard + "?", "Card Detection");
                                                pictureBoxPlayerHandPredict.Image = Image.FromFile(CardIconFilePath + @"\" + DrawnPredictedCard + CardIconExtention);

                                                if (TimerProgressBarTicked)
                                                {
                                                    if (progressBarRemoteCardVerification.Value >= progressBarRemoteCardVerification.Maximum)
                                                    {
                                                        progressBarRemoteCardVerification.Value = 0;
                                                        TimerCardVerificationPGElapsed = true;
                                                    }
                                                    else
                                                    {
                                                        progressBarRemoteCardVerification.Invoke(new Action(() =>
                                                        {
                                                            progressBarRemoteCardVerification.Value += 1;
                                                            progressBarRemoteCardVerification.Update();
                                                        }));
                                                    }
                                                    TimerProgressBarTicked = false;
                                                }
                                            }

                                            if (TimerCardVerificationPGElapsed)//If Card Approval sesson ended
                                            {
                                                DrawnCards++;
                                                Console.WriteLine($"PlayersTurn: {PlayersTurn}");
                                                if (!comboBoxRemoteManualCardEntry.Visible)//If there was no overide
                                                {
                                                    Console.WriteLine($"PlayersTurn: {PlayersTurn} at {PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())}");

                                                    PlayerData[PlayersTurn].Insert(PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString()), DrawnPredictedCardID.ToString());
                                                    PrintListOfStringLists(PlayerData);
                                                }
                                                pictureBoxPlayerHandPredict.Image = null;
                                                buttonRemoteCardVerifyYES.Visible = false;
                                                buttonRemoteCardVerifyNO.Visible = false;
                                                progressBarRemoteCardVerification.Visible = false;
                                                comboBoxRemoteManualCardEntry.Visible = false;
                                                buttonRemoteCardCorrectionEnter.Visible = false;
                                                CardPredicted = false;
                                                DrawCardCompleted = false;
                                                DrawButtonEnable = true;
                                                TimerCardVerificationPGElapsed = false;
                                                RobotRetrievingCard = false;
                                                UpdatePlayerDataDependants();
                                            }
                                        }

                                        //Add to player turn count if player has 4 cards
                                        if (DrawnCards == 4)
                                        {
                                            Console.WriteLine($"Player drew 4 cards");
                                            //For the next turn ignore special cards efects since they would have already been spent
                                            PlayerDrawingTwoOrFoInitializeVars = false;
                                            RemotePlayerDrawFourAskColorStep = true;
                                        }

                                        //Step 2 ASK COLOR 
                                        if (RemotePlayerDrawFourAskColorStep)
                                        {
                                            //check if color has already been solved
                                            if (treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text == "Wild Draw 4")
                                            {//then we need to find out color
                                                if (HumanChooseColorAskState)
                                                {//prompt human for color selection 
                                                    Console.WriteLine("Wild draw 4");
                                                    RemoteChatSend(GetRandomCardColorPrompts(), "System");//whats the color?
                                                    HumanChooseColorAskState = false;
                                                    HumanChooseColorWaitForResponceState = true;
                                                }
                                                if (HumanChooseColorRepeatColorChosenState)
                                                {
                                                    Console.WriteLine("Asked for color verification");
                                                    //define string to plug in user responce
                                                    ChosenColorVerificationMessages = new string[]
                                                    {
                                            $"Did you choose {HumanChooseColor} for your wild card?",
                                            $"Is the color you selected for the wild card {HumanChooseColor}?",
                                            $"Confirm if your wild card color is {HumanChooseColor}.",
                                            $"Is the wild card color you picked {HumanChooseColor}?",
                                            $"Are you confirming {HumanChooseColor} as your wild card color?",
                                            $"Did you decide on {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the color of your chosen wild card?",
                                            $"Confirm that the wild card color you chose is {HumanChooseColor}.",
                                            $"Are you going with the color {HumanChooseColor} for your wild card?",
                                            $"Is {HumanChooseColor} the designated color for your wild card?",
                                            $"Confirm your choice: is it the color {HumanChooseColor} for your wild card?",
                                            $"Did you pick {HumanChooseColor} as the color for your wild card?",
                                            $"Is your wild card color officially {HumanChooseColor}?",
                                            $"Affirm if the color of your wild card is {HumanChooseColor}.",
                                            $"Did you finalize {HumanChooseColor} as your wild card color?",
                                            $"Is {HumanChooseColor} the color you've chosen for your wild card?",
                                            $"Can you confirm that the wild card color is {HumanChooseColor}?",
                                            $"Did you settle on {HumanChooseColor} for your wild card?",
                                            $"Is the color {HumanChooseColor} officially chosen for your wild card?",
                                            $"Confirm that {HumanChooseColor} is the color you've chosen for your wild card."
                                                    };

                                                    RemoteChatSend(GetRandomChosenColorVerificationMessages(), "System");// the color is "red"?  Exception thrown: 'System.InvalidOperationException' in System.Speech.dll

                                                    HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                    HumanChooseColorRepeatColorChosenState = false;
                                                    HumanChooseColorWaitForConformationStateInitializeVars = false;
                                                    HumanChooseColorWaitForConformationState = true;
                                                }
                                                if (HumanChooseColorPromptCorrectResponceState)//HumanChooseColorRepeatColorChosenState
                                                {
                                                    RemoteChatSend(GetRandomMisunderstoodColorResponce(), "System");//oops my bad what color is it 

                                                    HumanChooseColorWaitForConformationStartTime = Environment.TickCount;
                                                    HumanChooseColorWaitForResponceState = true;
                                                    HumanChooseColorPromptCorrectResponceState = false;
                                                }
                                                if (HumanChooseColorWaitForConformationState)
                                                {
                                                    if (!HumanChooseColorWaitForConformationStateInitializeVars)
                                                    {
                                                        Console.WriteLine("Waiting for conformation");
                                                        HumanChooseColorWaitForConformationStateInitializeVars = true;
                                                    }

                                                    if (Environment.TickCount > HumanChooseColorWaitForConformationStartTime + HumanChooseColorWaitForConformationTime)
                                                    {
                                                        HumanChooseColorWaitForConformationState = false;
                                                        HumanChooseColorExicuteResponceState = true;
                                                    }
                                                }
                                                if (HumanChooseColorExicuteResponceState)
                                                {
                                                    Console.WriteLine("Wild draw 4 Card Exicuted");
                                                    treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text = HumanChooseColor;
                                                    DiscardCardID = Array.IndexOf(UNOCards, HumanChooseColor);
                                                    UpdatePlayerDataDependants(true);
                                                    PlayerIgnoreCardConsequenses = true;
                                                    PreviousPlayerPlayedCard = false;
                                                    RemotePlayerTurnEndCall = true;
                                                    HumanChooseColorExicuteResponceState = false;
                                                }
                                            }
                                            else
                                            {//Treat wild card after color solve
                                                Console.WriteLine("Wild draw 4 Card Exicuted");
                                                DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                                                UpdatePlayerDataDependants(true);
                                                PlayerIgnoreCardConsequenses = true;
                                                PreviousPlayerPlayedCard = false;
                                                RemotePlayerTurnEndCall = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {//all other cards
                                    UpdatePlayerDataDependants(true);

                                    CardPlayabilityFound = true;
                                }
                            }
                        }
                        else //(!PreviousPlayerPlayedCard)
                        {//card is the same as last turn, Find card id unless last tree view is a solved wild, then dont overide solved color
                            if (!DiscardCardIDFound)
                            {
                                Console.WriteLine("card is the same as last turn, Find card id unless last tree view is a solved wild, then dont overide solved color");
                                labelLoadingPlayerHandPredict.Visible = true;
                                labelLoadingPlayerHandPredict.Update();
                                var Card = PredictCard(0);
                                DiscardCardID = Card.CardID;
                                //Update Dicard Prediction Preview
                                string imagePath = CardIconFilePath + @"\" + Card.PredictedCard + CardIconExtention;
                                pictureBoxDiscardPrediction.Image = Image.FromFile(imagePath);
                                Console.WriteLine($"Last Discard Card = {treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text}");
                                if ((treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Blue"
                                    && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Green"
                                    && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Red"
                                    && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Yellow"))
                                {//if the card is not a solved wild card then add the found card 
                                    Console.WriteLine($"the card is not a solved wild card, add the found card: {UNOCards[DiscardCardID]}");
                                    treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);
                                }
                                else
                                {//if the card is a solved wild card, keep the color found
                                    Console.WriteLine($"the card({UNOCards[DiscardCardID]}) is a solved wild card, keep the color found: {treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text}");
                                    //treeViewDiscardPileHand.Nodes.Add(treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1]);
                                    DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                                }
                                PlayerCanTaunt = true;
                                RemotePlayerSelectedCardIndex = RemotePlayerSelectedCardNullValue;
                                UpdatePlayerDataDependants();
                                DiscardCardIDFound = true;
                                CardPlayabilityFound = true;
                            }
                        }

                        if (CardPlayabilityFound)
                        {
                            if (!CardPlayabilityFoundInitializeVars)
                            {//update players hand
                                UpdatePlayerDataDependants(true);
                                //For the next turn ignore special cards efects since they would have already been spent
                                PlayerIgnoreCardConsequenses = true;
                                CardPlayabilityFoundInitializeVars = true;
                            }

                            if (DrawCardCompleted)
                            {
                                //Drew Card
                                if (!CardPredicted)//Only Run this once
                                {
                                    labelLoadingPlayerHandPredict.Visible = true;
                                    labelLoadingPlayerHandPredict.Update();
                                    var Card = PredictCard(GetLastIntFromString(PlayerData[PlayersTurn][0]));
                                    DrawnPredictedCard = Card.PredictedCard.ToString();
                                    DrawnPredictedCardID = Card.CardID;
                                    if (Card.PredictedCard.ToString() == "UNO")
                                    {
                                        DrawnPredictedCard = "Blue 0";
                                    }
                                    if (Card.CardID == 52)
                                    {
                                        DrawnPredictedCardID = 0;
                                    }
                                    if (DrawnPredictedCard != "")
                                    {
                                        progressBarRemoteCardVerification.Value = 0;
                                        CardPredicted = true;
                                        Console.WriteLine($"CardPredicted {DrawnPredictedCard}");
                                    }
                                }
                                if (CardPredicted && comboBoxRemoteManualCardEntry.Visible == false)
                                {
                                    labelLoadingPlayerHandPredict.Visible = false;
                                    buttonRemoteCardVerifyYES.Visible = true;
                                    buttonRemoteCardVerifyNO.Visible = true;
                                    progressBarRemoteCardVerification.Visible = true;
                                    //RemoteChatSend("This is a " + DrawnPredictedCard + "?", "Card Detection");
                                    pictureBoxPlayerHandPredict.Image = Image.FromFile(CardIconFilePath + @"\" + DrawnPredictedCard + CardIconExtention);

                                    if (TimerProgressBarTicked)
                                    {
                                        if (progressBarRemoteCardVerification.Value >= progressBarRemoteCardVerification.Maximum)
                                        {
                                            progressBarRemoteCardVerification.Value = 0;
                                            TimerCardVerificationPGElapsed = true;
                                        }
                                        else
                                        {
                                            progressBarRemoteCardVerification.Invoke(new Action(() =>
                                            {
                                                progressBarRemoteCardVerification.Value += 1;
                                                progressBarRemoteCardVerification.Update();
                                            }));
                                        }
                                        TimerProgressBarTicked = false;
                                    }
                                }

                                if (TimerCardVerificationPGElapsed)//If Card Approval sesson ended
                                {
                                    Console.WriteLine($"Inserted: {DrawnPredictedCardID.ToString()}");
                                    if (!comboBoxRemoteManualCardEntry.Visible)//If there was no overide
                                    {
                                        Console.WriteLine($"PlayersTurn: {PlayersTurn} at {PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())}");

                                        PlayerData[PlayersTurn].Insert(PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString()), DrawnPredictedCardID.ToString());
                                        PrintListOfStringLists(PlayerData);
                                    }
                                    pictureBoxPlayerHandPredict.Image = null;
                                    buttonRemoteCardVerifyYES.Visible = false;
                                    buttonRemoteCardVerifyNO.Visible = false;
                                    progressBarRemoteCardVerification.Visible = false;
                                    comboBoxRemoteManualCardEntry.Visible = false;
                                    buttonRemoteCardCorrectionEnter.Visible = false;
                                    CardPredicted = false;
                                    DrawCardCompleted = false;
                                    DrawButtonEnable = true;
                                    TimerCardVerificationPGElapsed = false;
                                    RemotePlayerDrewCardPlayableCheck = true;
                                    RobotRetrievingCard = false;
                                    UpdatePlayerDataDependants();
                                }
                            }
                            if (RomotePlayCard)
                            {
                                if (!PlayerPlayedCardIDFound)
                                {
                                    PlayerCardPlayingID = 0;
                                    int CardPlayingTerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());
                                    if ((RemotePlayerSelectedCardIndex + 2) >= CardPlayingTerminatorIndex)
                                    {//selected card in after the terminator and we need to add one to the index
                                        Console.WriteLine($"Selected Card PlayerData Index = {(RemotePlayerSelectedCardIndex + 2)} >= terminator Index = {CardPlayingTerminatorIndex}");
                                        Console.WriteLine($"Player Playing: {PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 3]}");
                                        PlayerCardPlayingID = int.Parse(PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 3]);
                                        Console.WriteLine($"CardPlaying: {PlayerCardPlayingID}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Selected Card PlayerData Index = {(RemotePlayerSelectedCardIndex + 2)} < terminator Index = {CardPlayingTerminatorIndex}");
                                        Console.WriteLine($"Player Playing: {PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 2]}");
                                        PlayerCardPlayingID = int.Parse(PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 2]);
                                        Console.WriteLine($"CardPlaying: {PlayerCardPlayingID}");
                                    }
                                    if (PlayerCardPlayingID >= 40 && PlayerCardPlayingID <= 54)
                                    {
                                        PlayerIgnoreCardConsequenses = false;
                                        Console.WriteLine($"Player playing special card, PlayerIgnoreCardConsequenses = {PlayerIgnoreCardConsequenses}");
                                    }
                                    PlayerPlayedCardIDFound = true;
                                }
                                if (PlayerCardPlayingID == 53 || PlayerCardPlayingID == 54)
                                {//If player is playing a wild card thet they need to select color of
                                    if (PlayedCardColorChosen == false)
                                    {
                                        if (!PlayedCardColorChosenInitVars)
                                        {
                                            Console.WriteLine($"Player Needs to chose Color of wild card");
                                            textBoxRemoteChat.Text = "Send Chosen Color Here!";
                                            PlayedCardColorChosenInitVars = true;
                                        }
                                        if (labelRemoteChat.Text == "Blue" || labelRemoteChat.Text == "blue" || labelRemoteChat.Text == "blu" || labelRemoteChat.Text == "BLUE" || labelRemoteChat.Text == "b" || labelRemoteChat.Text == "B")
                                        {//If color sent through chat
                                            treeViewDiscardPileHand.Nodes.Add("Blue");
                                            PlayedCardColorChosen = true;
                                            //reset bool
                                            PlayedCardColorChosenInitVars = false;
                                        }
                                        if (labelRemoteChat.Text == "Green" || labelRemoteChat.Text == "green" || labelRemoteChat.Text == "grn" || labelRemoteChat.Text == "GREEN" || labelRemoteChat.Text == "g" || labelRemoteChat.Text == "G")
                                        {//If color sent through chat
                                            treeViewDiscardPileHand.Nodes.Add("Green");
                                            PlayedCardColorChosen = true;
                                            //reset bool
                                            PlayedCardColorChosenInitVars = false;
                                        }
                                        if (labelRemoteChat.Text == "Red" || labelRemoteChat.Text == "red" || labelRemoteChat.Text == "rd" || labelRemoteChat.Text == "RED" || labelRemoteChat.Text == "r" || labelRemoteChat.Text == "R")
                                        {//If color sent through chat
                                            treeViewDiscardPileHand.Nodes.Add("Red");
                                            PlayedCardColorChosen = true;
                                            //reset bool
                                            PlayedCardColorChosenInitVars = false;
                                        }
                                        if (labelRemoteChat.Text == "Yellow" || labelRemoteChat.Text == "yellow" || labelRemoteChat.Text == "yel" || labelRemoteChat.Text == "YELLOW" || labelRemoteChat.Text == "y" || labelRemoteChat.Text == "Y")
                                        {//If color sent through chat
                                            treeViewDiscardPileHand.Nodes.Add("Yellow");
                                            PlayedCardColorChosen = true;
                                            //reset bool
                                            PlayedCardColorChosenInitVars = false;
                                        }
                                    }
                                    //If its players first time playing wild card show instructional prompt
                                    if (!RemotePlayerPlayedFirstWildCard)
                                    {
                                        MessageBox.Show("Enter Color in the Message Box Above");

                                        RemotePlayerPlayedFirstWildCard = true;
                                    }
                                }
                                else
                                {//If player is not playing a wild card thet they need to select color of
                                    PlayedCardColorChosen = true;
                                }
                                    
                                if (PlayedCardColorChosen)
                                {
                                    //Played Card
                                    int TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());

                                    if (RemotePlayerSelectedCardIndex + 2 < TerminatorIndex)
                                    {
                                        //Left Pile
                                        if (!RemoteShuffelSigSent)
                                        {
                                            Console.WriteLine($"Player Data:");
                                            PrintListOfStringLists(PlayerData);
                                            Console.WriteLine($"Playing Card In Left Pile. RemotePlayerSelectedCardIndex=({RemotePlayerSelectedCardIndex}), TerminatorIndex=({TerminatorIndex})");
                                            SendShuffleSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 2, Math.Abs((RemotePlayerSelectedCardIndex + 2) - TerminatorIndex));
                                            Console.WriteLine($"Coil {((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 2} run for {Math.Abs((RemotePlayerSelectedCardIndex + 2) - TerminatorIndex)} Iterations whith abs of: {RemotePlayerSelectedCardIndex + 2} - {TerminatorIndex} for the left pile");
                                            RemoteShuffelSigSent = true;
                                        }

                                        if (ShuffleCardCompleted)//RemoteTurnOver
                                        {
                                            //Console.WriteLine($"Remove {Array.IndexOf(UNOCards, PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 2])} at {RemotePlayerSelectedCardIndex + 2}");
                                            //PlayerData[PlayersTurn].RemoveAt(RemotePlayerSelectedCardIndex + 2);
                                            PrintListOfStringLists(PlayerData);
                                            Console.WriteLine($"{RemotePlayerSelectedCardIndex} is before the index of {PlayerDataTerminator} ({TerminatorIndex}).");
                                            PreviousPlayerPlayedCard = true;
                                            RemotePlayerTurnEndCall = true;
                                            RobotPlayingCard = false;
                                        }
                                    }
                                    else if (RemotePlayerSelectedCardIndex + 2 >= TerminatorIndex)
                                    {
                                        //Right Pile
                                        if (!RemoteShuffelSigSent)
                                        {
                                            Console.WriteLine($"Player Data:");
                                            PrintListOfStringLists(PlayerData);
                                            Console.WriteLine($"Card In Right Pile. RemotePlayerSelectedCardIndex=({RemotePlayerSelectedCardIndex}), TerminatorIndex=({TerminatorIndex})");
                                            SendShuffleSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 3, Math.Abs((RemotePlayerSelectedCardIndex + 3) - TerminatorIndex));
                                            Console.WriteLine($"Coil {((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 3} run for {Math.Abs((RemotePlayerSelectedCardIndex + 3) - TerminatorIndex)} Iterations whith abs of: {RemotePlayerSelectedCardIndex + 3} - {TerminatorIndex} for the right pile");
                                            RemoteShuffelSigSent = true;
                                        }

                                        if (ShuffleCardCompleted)
                                        {
                                            //Console.WriteLine($"Remove {Array.IndexOf(UNOCards, PlayerData[PlayersTurn][RemotePlayerSelectedCardIndex + 3])} at {RemotePlayerSelectedCardIndex + 3}");
                                            //PlayerData[PlayersTurn].RemoveAt(RemotePlayerSelectedCardIndex + 3);
                                            PrintListOfStringLists(PlayerData);
                                            Console.WriteLine($"{RemotePlayerSelectedCardIndex} is after the index of {PlayerDataTerminator} ({TerminatorIndex}).");
                                            PreviousPlayerPlayedCard = true;
                                            RemotePlayerTurnEndCall = true;
                                            RobotPlayingCard = false;
                                        }
                                    }
                                }
                            }
                        }

                        //check if drew card is playable else end the turn
                        if (RemotePlayerDrewCardPlayableCheck)
                        {
                            //for (int i = 0; i < flowLayoutPanelPlayersCards.Controls.Count; i++)
                            //{
                            //    if (int.TryParse(PlayerData[PlayersTurn][i + 2], out int index))
                            //    {
                            //        if ((PlayerData[PlayersTurn].IndexOf(index.ToString()) != -1 && PlayerData[PlayersTurn].IndexOf(index.ToString()) >= PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())))
                            //        {//If Card is at the terminator index or after it
                            //            index = int.Parse(PlayerData[PlayersTurn][i + 3]);
                            //        }
                            //        if (CardVCard.Count > index && CardVCard[index].Contains(DiscardCardID))
                            //        {
                            //            Console.WriteLine($"{index} is compatible with {DiscardCardID}");
                            //            flowLayoutPanelPlayersCards.Controls[i].Enabled = true;  //.BackColor = Color.Black;
                            //        }
                            //        else
                            //        {
                            //            Console.WriteLine($"{index} is not compatible with {DiscardCardID}");
                            //            ChangeImageAt(flowLayoutPanelPlayersCards, i, GetDimmedImage(GetImageAt(flowLayoutPanelPlayersCards, i)));
                            //            flowLayoutPanelPlayersCards.Controls[i].Enabled = false;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        Console.WriteLine($"Unable to parse '{PlayerData[PlayersTurn][i + 2]}' as an integer for compatibility check.");
                            //    }
                            //}
                            UpdatePlayerDataDependants(true);

                            RemotePlayerDrewCardPlayableCheck = false;

                            if (!flowLayoutPanelPlayersCards.Controls.OfType<Control>().Any(control => control.Enabled))
                            {//drawn card not playable, end turn
                                PreviousPlayerPlayedCard = false;
                                RemotePlayerTurnEndCall = true;
                            }
                        }

                        
                    }

                    //Second turn of the game, flip first card?  FirstCardPlayed
                    if (int.Parse(PlayerData[PlayersTurn][1]) == 1 && !FirstCardPlayed)
                    {
                        if (!RemotePlayerPlayingFirstCardInitializeVars)
                        {
                            Console.WriteLine("Remote Player, Playing First Card");
                            RemotePlayerPlayingFirstCardInitializeVars = true;
                        }

                        if (DrawCardCompleted)
                        {//once player draws card, play the card
                            if (!RemoteShuffelSigSent)
                            {//imediatly place card after draw
                                Console.WriteLine("Remote Player drew card Card, placing card...");
                                SendShuffleSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 2, 1, false);
                                RemoteShuffelSigSent = true;
                            }
                            if (ShuffleCardCompleted)
                            {//advance turn
                                RemotePlayerPlayingFirstCardInitializeVars = false;
                                PreviousPlayerPlayedCard = true;
                                RemotePlayerTurnEndCall = true;
                                FirstCardPlayed = true;
                            }
                        }
                    }

                    if (RemotePlayerTurnEndCall)
                    {//reset all vars after turn
                        PlayerCardPlayingID = 0;
                        CheckForUno = false;
                        PlayerCanTaunt = false;
                        PlayerPlayedCardIDFound = false;
                        ShuffleCardCompleted = false;
                        DrawCardCompleted = false;
                        DiscardCardIDFound = false;
                        CardPlayabilityFound = false;
                        RomotePlayCard = false;
                        RemotePlayerSelectedCardIndex = RemotePlayerSelectedCardNullValue;
                        RemoteShuffelSigSent = false;
                        RemotePlayerDrawingTwoOrFour = false;
                        CardPlayabilityFoundInitializeVars = false;
                        PlayedCardColorChosen = false;
                        RemotePlayerDrawFourAskColorStep = false;
                        HumanChooseColorAskState = true;
                        Console.WriteLine("Check if Game Won");
                        Console.WriteLine("Player Data: ");
                        PrintListOfStringLists(PlayerData);
                        Console.WriteLine($"int.Parse(PlayerData[PlayersTurn][1]) = {int.Parse(PlayerData[PlayersTurn][1])} > 0? = {int.Parse(PlayerData[PlayersTurn][1]) > 0}");
                        Console.WriteLine($"FirstCardPlayed = {FirstCardPlayed} == true? = {FirstCardPlayed}");
                        Console.WriteLine($"PlayerData[PlayersTurn].Count = {PlayerData[PlayersTurn].Count} <= 3? = {PlayerData[PlayersTurn].Count <= 3}");
                        if (int.Parse(PlayerData[PlayersTurn][1]) > 0 && FirstCardPlayed && PlayerData[PlayersTurn].Count <= 3)
                        {//Check if game won
                            Console.WriteLine($"Player {PlayersTurn} won!");
                            Console.WriteLine("Player Data: ");
                            PrintListOfStringLists(PlayerData);
                            Console.WriteLine($"Turn Count: {PlayerData[PlayersTurn].Count}");
                            SendTauntSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 4);
                            SendTauntSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 5);
                            GameLive = false;
                        }
                        //Add to player turn count 
                        PlayerData[PlayersTurn][1] = (int.Parse(PlayerData[PlayersTurn][1]) + 1).ToString();
                        pictureBoxDiscardPile.BackColor = Color.Black;
                        RemotePlayerTurnEndCall = false;
                        RobotRetrievingCard = false;
                        CardPredicted = false;
                        PrintListOfStringLists(PlayerData);
                        UpdatePlayerDataDependants();
                        AdvancePlayerTurn();
                    }

                    
                }

                //Console.WriteLine($"Human?, PlayerData.Count = {PlayerData.Count}, PlayersTurn = {PlayersTurn}");

                //If the Player Is The Human
                if (PlayerData[PlayersTurn][0] == "Human")
                {
                    pictureBoxDiscardPile.BackColor = Color.Black;
                    buttonSkipHuman.Enabled = true;
                    buttonSkipHuman.Visible = true;


                    //First turn of the game Draw Cards
                    if (PlayerData[PlayersTurn][1] == null || int.Parse(PlayerData[PlayersTurn][1]) == 0)
                    {
                        //draw 7 cards
                        if (!HumanDrawCardsMessageSent)
                        {
                            Console.WriteLine("Human First turn of the game Draw Cards");

                            RemoteChatSend(GetRandomCardDrawInstruction(), "System");
                            HumanDrawCardsMessageSent = true;
                            progressBarHumanDrawTime.Value = 0;
                        }
                        if (HumanDrawCardsMessageSent)
                        {
                            progressBarHumanDrawTime.Visible = true;
                            buttonSkipHumanDrawCards.Visible = true;
                            buttonSkipHumanDrawCards.Enabled = true;
                            //Console.WriteLine("buttonSkipHumanDrawCards.Visible = true");

                            if (TimerProgressBarTicked)
                            {
                                if (progressBarHumanDrawTime.Value >= progressBarHumanDrawTime.Maximum)
                                {
                                    progressBarHumanDrawTime.Value = 0;
                                    HumanDrawCardsComplete = true;
                                }
                                else
                                {
                                    progressBarHumanDrawTime.Invoke(new Action(() =>
                                    {
                                        progressBarHumanDrawTime.Value += 1;
                                        progressBarHumanDrawTime.Update();
                                    }));
                                }
                                TimerProgressBarTicked = false;
                            }
                        }

                        if (HumanDrawCardsComplete)
                        {
                            PlayerData[PlayersTurn].Add(StartingCardsCount.ToString());
                            Console.WriteLine("Human Draw Cards");
                            progressBarHumanDrawTime.Visible = false;
                            buttonSkipHumanDrawCards.Visible = false;
                            buttonSkipHumanDrawCards.Enabled = false;
                            Console.WriteLine("buttonSkipHumanDrawCards.Visible = false");
                            PreviousPlayerPlayedCard = false;
                            HumanTurnEndCall = true;
                            HumanDrawCardsComplete = false;
                        }
                    }

                    //All Other Turns exept first card play
                    else if (int.Parse(PlayerData[PlayersTurn][1]) > 0 && FirstCardPlayed)
                    {
                        if (!HumanTurnInitializeVars)
                        {//print to terminal once to prevent terminal flodding 
                            Console.WriteLine($"Human's Turn");
                            Console.WriteLine($"Human int.Parse(PlayerData[PlayersTurn][1]): {int.Parse(PlayerData[PlayersTurn][1])} > 0 and FirstCardPlayed:({FirstCardPlayed}) == true");
                            Console.WriteLine($"PlayerIgnoreCardConsequenses = {PlayerIgnoreCardConsequenses}");

                            //Remind Human thet its their turn
                            //RemoteChatSend(GetRemindHumanTurnResponse(), "System");

                            //record Discard pile before turn to compare to after turn for draw detection
                            Bitmap DiscardPileImageFull = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
                            Graphics.FromImage(DiscardPileImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);

                            // Convert the Bitmap to an Image<Bgr, Byte>
                            HumanTurnPreImage = DiscardPileImageFull.ToMat();

                            HumanTurnInitializeVars = true;
                        }

                        if (!HumanPreTurnTableStable)
                        {
                            HumanPreTurnTakenPhotos = true;
                        }

                        if (HumanPreTurnTableStable)
                        {
                            //Console.WriteLine($"Human int.Parse(PlayerData[PlayersTurn][1]): {int.Parse(PlayerData[PlayersTurn][1])} > 0 and FirstCardPlayed:({FirstCardPlayed}) == true");
                            if (!PlayerIgnoreCardConsequenses)
                            {//Wild Cards
                                if (DiscardCardIDFound == false)
                                {
                                    labelLoadingPlayerHandPredict.Visible = true;
                                    labelLoadingPlayerHandPredict.Update();
                                    var Card = PredictCard(0);
                                    DiscardCardID = Card.CardID;
                                    InitDiscardID = Card.CardID;
                                    //Update Dicard Prediction Preview
                                    string imagePath = CardIconFilePath + @"\" + Card.PredictedCard + CardIconExtention;
                                    pictureBoxDiscardPrediction.Image = Image.FromFile(imagePath);
                                    Console.WriteLine($"DiscardCardID = {DiscardCardID}");
                                    if (treeViewDiscardPileHand.Nodes.Count > 0 && (DiscardCardID == 53 || DiscardCardID == 54))
                                    {//Only search in list if list has at leat one item
                                        Console.WriteLine($"Last Discard Card = {treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text}");
                                        if ((treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Blue"
                                            && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Green"
                                            && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Red"
                                            && treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text != "Yellow"))
                                        {//if the card is not a solved wild card then add the found card 
                                            Console.WriteLine($"the card is not a solved wild card, add the found card: {UNOCards[DiscardCardID]}");
                                            treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);
                                        }
                                        else
                                        {//if the card is a solved wild card, keep the color found
                                            Console.WriteLine($"the card({UNOCards[DiscardCardID]}) is a solved wild card, keep the color found: {treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text}");
                                            //treeViewDiscardPileHand.Nodes.Add(treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1]);
                                            DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                                        }
                                    }
                                    else
                                    {
                                        treeViewDiscardPileHand.Nodes.Add(UNOCards[DiscardCardID]);
                                    }
                                    UpdatePlayerDataDependants();
                                    PlayerSeeWildCardInitializeVars = false;
                                    DiscardCardIDFound = true;
                                }

                                if (DiscardCardIDFound && !CardPlayabilityFound)
                                {
                                    labelLoadingPlayerHandPredict.Visible = false;
                                    //Sense and exicute here effects of special cards 
                                    if (DiscardCardID >= 40 && !PlayerIgnoreCardConsequenses)
                                    {//If card has effects and special card has not spent effects
                                        if (!PlayerSeeWildCardInitializeVars)
                                        {//run Console feedback once to prevent terminal flodding 
                                            Console.WriteLine("special card has not spent effects, executing effects...");
                                            PlayerSeeWildCardInitializeVars = true;
                                        }
                                    
                                        if (DiscardCardID == 41 || DiscardCardID == 44 || DiscardCardID == 47 || DiscardCardID == 50)
                                        {//reverse
                                            Console.WriteLine("reverse");
                                            PrintListOfStringLists(PlayerData);
                                            UpdatePlayerDataDependants();
                                            ////change direction
                                            GameDirectionClockwise = !GameDirectionClockwise;

                                            //if (GameDirectionClockwise)
                                            //{
                                            //    GameDirectionClockwise = false;
                                            //}
                                            //else
                                            //{
                                            //    GameDirectionClockwise = true;
                                            //}

                                            //Skip next player to prevent reverse giving player additional turn
                                            //Skip next player to prevent reverse giving player additional turn
                                            if (Players.Count > 2)
                                            {
                                                //only if there are more than 2 players else make reverse act as skip card in 1v1
                                                SkipNextPlayerOnAdvanceTurn = true;
                                            }

                                            //For the next turn ignore special cards efects since they would have already been spent
                                            PreviousPlayerPlayedCard = false;
                                            HumanTurnEndCall = true;

                                            PlayerIgnoreCardConsequenses = true;
                                            //CardPlayabilityFound = true;
                                            //PlayerCanPlayCard = true;
                                        }
                                        else if (DiscardCardID == 42 || DiscardCardID == 45 || DiscardCardID == 48 || DiscardCardID == 51)
                                        {//skip
                                            Console.WriteLine("skip");
                                            PrintListOfStringLists(PlayerData);
                                            UpdatePlayerDataDependants();
                                            //For the next turn ignore special cards efects since they would have already been spent
                                            //skip player
                                            PreviousPlayerPlayedCard = false;
                                            HumanTurnEndCall = true;
                                        }
                                        else
                                        {
                                            if (!HumanTurnPromptSent)
                                            {
                                                //Remind Human thet its their turn
                                                RemoteChatSend(GetRemindHumanTurnResponse(), "System");
                                                HumanTurnPromptSent = true;
                                            }
                                        }

                                        if (DiscardCardID == 40 || DiscardCardID == 43 || DiscardCardID == 46 || DiscardCardID == 49)
                                        {//draw 2
                                            Console.WriteLine("draw 2");

                                            //Add two cards to human card count and remember that cards were added
                                            PlayerData[PlayersTurn][2] = (int.Parse(PlayerData[PlayersTurn][2]) + 2).ToString();
                                            HumanCardCountAdded = true;

                                            PlayerIgnoreCardConsequenses = true;
                                            CardPlayabilityFound = true;
                                            PlayerCanPlayCard = false;
                                        }
                                        else if (DiscardCardID == 55 || DiscardCardID == 56 || DiscardCardID == 57 || DiscardCardID == 58)
                                        {//Wild
                                            if (InitDiscardID == 53)
                                            {
                                                Console.WriteLine("Wild");
                                                CardPlayabilityFound = true;
                                                PlayerCanPlayCard = true;
                                            }
                                            else if (InitDiscardID == 54)
                                            {//Wild draw 4
                                                Console.WriteLine("Wild draw 4");

                                                //Add two cards to human card count and remember that cards were added
                                                PlayerData[PlayersTurn][2] = (int.Parse(PlayerData[PlayersTurn][2]) + 4).ToString();
                                                HumanCardCountAdded = true;

                                                CardPlayabilityFound = true;
                                                PlayerCanPlayCard = false;

                                            }
                                        }
                                        //else if (DiscardCardID == 54)
                                        //{//Wild draw 4
                                        //    Console.WriteLine("Wild draw 4");
                                        //    CardPlayabilityFound = true;
                                        //    PlayerCanPlayCard = false;

                                        //}
                                    }
                                }
                                PlayerIgnoreCardConsequenses = true;
                            }
                            else
                            {
                                if (!HumanTurnPromptSent)
                                {
                                    //Remind Human thet its their turn
                                    RemoteChatSend(GetRemindHumanTurnResponse(), "System");
                                    HumanTurnPromptSent = true;
                                }
                            }
                            if (CardPlayabilityFound || PlayerIgnoreCardConsequenses)
                            {
                                if (!LightScreenSawTurn)
                                {
                                    ReadLightScreen();
                                }

                                if (LightScreenSawTurn && !HumanMotionDetectorVarsReset)
                                {
                                    //MotionInsideDicardPile = 0;
                                    MotionOutsideDiscardPile = 0;
                                    int StartTime = Environment.TickCount;
                                    HumanMotionDetectorVarsReset = true;

                                    Console.WriteLine("HumanMotionDetectorVarsReset");
                                }

                                //Console.WriteLine($"Environment.TickCount = {Environment.TickCount}, startTime = {startTime}, startTime + HumanMotionCoolDownTime = {startTime * HumanMotionCoolDownTime}");

                                if (HumanMotionDetectorVarsReset && Environment.TickCount > startTime + HumanMotionCoolDownTime)
                                {//If motion cool down time is up 
                                    // Create a new Bitmap from the PictureBox
                                    Bitmap DiscardPileImageFull = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
                                    Graphics.FromImage(DiscardPileImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);

                                    // Convert the Bitmap to an Image<Bgr, Byte>
                                    HumanTurnPostImage = DiscardPileImageFull.ToMat();

                                    //compare pre and post images to see if theyre differnt: is a card was played
                                    // Call the method to compare the loaded images
                                    bool DiscardPileIsDifferent = AreImagesDifferent(HumanTurnPreImage, HumanTurnPostImage, HumanPlayCardDetectionImageComparisonThreshold);

                                    Console.WriteLine("motion cool down time is up");
                                    //Console.WriteLine($"MotionInsideDicardPile = {MotionInsideDicardPile}, DiscardPileMotionDrawThreshold = {DiscardPileMotionDrawThreshold}");
                                    if (DiscardPileIsDifferent) //if (PlayerCanPlayCard && MotionInsideDicardPile >= DiscardPileMotionDrawThreshold)
                                    {//Player Played Card
                                        Console.WriteLine("Player Played Card");
                                        PreviousPlayerPlayedCard = true;
                                        PlayerIgnoreCardConsequenses = false;
                                        if (!HumanCardCountAdded)
                                        {
                                            //Player Played Card So Remove Card From Count
                                            PlayerData[PlayersTurn][2] = (int.Parse(PlayerData[PlayersTurn][2]) - 1).ToString();
                                        }
                                    }
                                    else
                                    {//player Drew Card
                                        Console.WriteLine("player Drew Card");
                                        PreviousPlayerPlayedCard = false;
                                        PlayerIgnoreCardConsequenses = true;
                                        if (!HumanCardCountAdded)
                                        {
                                            //Player Drew Card So Add Card To Count
                                            PlayerData[PlayersTurn][2] = (int.Parse(PlayerData[PlayersTurn][2]) + 1).ToString();
                                        }
                                    }
                                    //turn end
                                    HumanTurnEndCall = true;
                                }
                            }
                        }

                    }

                    //Second turn of the game, flip first card?  FirstCardPlayed
                    if (int.Parse(PlayerData[PlayersTurn][1]) == 1 && !FirstCardPlayed)
                    {
                        Console.WriteLine($"Human int.Parse(PlayerData[PlayersTurn][1]): {int.Parse(PlayerData[PlayersTurn][1])} == 1 and FirstCardPlayed:({FirstCardPlayed}) == false");

                        if (!HumanSendPlayFirstCardInstruction)
                        {
                            // Create a new Bitmap from the PictureBox
                            Bitmap DiscardPileImageFull = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
                            Graphics.FromImage(DiscardPileImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);

                            // Convert the Bitmap to an Image<Bgr, Byte>
                            HumanTurnPreImage = DiscardPileImageFull.ToMat();

                            Console.WriteLine("Human, Playing First Card");
                            RemoteChatSend(GetRandomPlaceFirstCardInstruction(), "System");
                            HumanSendPlayFirstCardInstruction = true;
                        }
                        if (HumanSendPlayFirstCardInstruction)
                        {//wait for human to draw and play card
                            if (!LightScreenSawTurn)
                            {
                                ReadLightScreen();
                            }

                            if (LightScreenSawTurn && !HumanMotionDetectorVarsReset)
                            {
                                //MotionInsideDicardPile = 0;
                                MotionOutsideDiscardPile = 0;
                                int StartTime = Environment.TickCount;
                                HumanMotionDetectorVarsReset = true;

                                Console.WriteLine("HumanMotionDetectorVarsReset");
                            }

                            //Console.WriteLine($"Environment.TickCount = {Environment.TickCount}, startTime = {startTime}, startTime + HumanMotionCoolDownTime = {startTime * HumanMotionCoolDownTime}");

                            if (HumanMotionDetectorVarsReset && Environment.TickCount > startTime + HumanMotionCoolDownTime)
                            {//If motion cool down time is up 
                                // Create a new Bitmap from the PictureBox
                                Bitmap DiscardPileImageFull = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
                                Graphics.FromImage(DiscardPileImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);

                                // Convert the Bitmap to an Image<Bgr, Byte>
                                HumanTurnPostImage = DiscardPileImageFull.ToMat();

                                //compare pre and post images to see if theyre differnt: is a card was played
                                // Call the method to compare the loaded images
                                bool DiscardPileIsDifferent = AreImagesDifferent(HumanTurnPreImage, HumanTurnPostImage, HumanPlayCardDetectionImageComparisonThreshold);

                                Console.WriteLine("motion cool down time is up");
                                //Console.WriteLine($"MotionInsideDicardPile = {MotionInsideDicardPile}, DiscardPileMotionDrawThreshold = {DiscardPileMotionDrawThreshold}");
                                if (DiscardPileIsDifferent)//if (MotionInsideDicardPile >= DiscardPileMotionDrawThreshold)
                                {//Player Played Card
                                    Console.WriteLine("Player Played Card");

                                    //end turn
                                    PreviousPlayerPlayedCard = true;
                                    HumanTurnEndCall = true;
                                    FirstCardPlayed = true;
                                }
                                else
                                {//player Drew Card
                                    Console.WriteLine("player Drew Card");

                                }
                            }
                        }
                    }
                    
                    if (HumanTurnEndCall)
                    {
                        HumanPreTurnTableStable = false;
                        CheckForUno = false;
                        HumanCardCountAdded = false;
                        buttonSkipHuman.Enabled = false;
                        buttonSkipHuman.Visible = false;
                        //Add to player turn count 
                        PlayerData[PlayersTurn][1] = (int.Parse(PlayerData[PlayersTurn][1]) + 1).ToString();
                        HumanTurnPromptSent = false;
                        LightScreenSawTurn = false;
                        HumanMotionDetectorVarsReset = false;
                        HumanTurnEndCall = false;
                        PlayerCanPlayCard = true;
                        HumanTurnInitializeVars = false;
                        DiscardCardIDFound = false;
                        HumanSendPlayFirstCardInstruction = false;
                        CardPlayabilityFound = false;
                        PrintListOfStringLists(PlayerData);
                        UpdatePlayerDataDependants();
                        AdvancePlayerTurn();
                    }
                }
            }
        }

        private bool AreImagesDifferent(Mat mat1, Mat mat2, int threshold)
        {
            // Check if the images have the same size
            if (mat1 == null || mat2 == null || mat1.Size != mat2.Size)
            {
                return true;
            }

            // Apply Gaussian blur to the images
            CvInvoke.GaussianBlur(mat1, mat1, new Size(5, 5), 0);
            CvInvoke.GaussianBlur(mat2, mat2, new Size(5, 5), 0);

            pictureBox1.Image = mat1.ToBitmap();
            pictureBox2.Image = mat2.ToBitmap();

            // Compute the absolute difference between the two images
            Mat difference = new Mat();
            CvInvoke.AbsDiff(mat1, mat2, difference);

            //pictureBox3.Image = difference.ToBitmap();

            // Convert the difference image to grayscale
            Mat grayDifference = new Mat();
            CvInvoke.CvtColor(difference, grayDifference, ColorConversion.Bgr2Gray);

            pictureBox3.Image = grayDifference.ToBitmap();

            // Apply threshold to the grayscale difference image
            CvInvoke.Threshold(grayDifference, grayDifference, threshold, 255, ThresholdType.Binary);

            pictureBox4.Image = grayDifference.ToBitmap();

            // Check if the thresholded image contains any pixel values greater than 0
            return CvInvoke.CountNonZero(grayDifference) > 0;
        }

        static string GenerateRobotColorChoiceMessage(string colorChoice)
        {
            // Array of 20 different snobby responses
            string[] snobbyResponses = {
            $"Well, it seems {colorChoice} is the only choice for someone with taste like mine.",
            $"Of course, I'll choose {colorChoice}. Only the best suits my style.",
            $"How predictable. {colorChoice} is the only color that matches my sophistication.",
            $"Choosing {colorChoice}? I expected nothing less from someone of my caliber.",
            $"Obviously, I'll go with {colorChoice}. It complements my brilliance.",
            $"It's elementary. {colorChoice} is the color for those who appreciate excellence.",
            $"Naturally, I'll pick {colorChoice}. A choice as refined as myself.",
            $"Ah, the elegance of {colorChoice}. Just like me.",
            $"Selecting {colorChoice} is the closest one can get to my level of sophistication.",
            $"Why settle for anything less? {colorChoice} is the only choice for someone of my stature.",
            $"Choosing {colorChoice} is the only decision that aligns with my impeccable taste.",
            $"I suppose {colorChoice} will have to do. It's the least inferior option.",
            $"Naturally, I'd choose {colorChoice}. It's the color of success.",
            $"One could only expect that I'd choose {colorChoice}. The color of distinction.",
            $"I'll graciously choose {colorChoice}. A decision made with my discerning eye.",
            $"Clearly, {colorChoice} is the color that befits someone of my elevated status.",
            $"Why be ordinary? {colorChoice} is the only option for someone as extraordinary as myself.",
            $"Ah, the sophistication of {colorChoice}. A choice suitable for a person of my caliber.",
            $"How original. {colorChoice} is my color, naturally.",
            $"Indubitably, {colorChoice} is the only color that complements my aura of excellence."
        };

            // Generate a random index to select a snobby response
            Random random = new Random();
            int index = random.Next(snobbyResponses.Length);

            // Return the selected snobby response
            return snobbyResponses[index];
        }

        static string FindMostFrequentColor(List<string> strings)
        {
            Dictionary<string, int> colorCounts = new Dictionary<string, int>
        {
            { "Blue", 0 },
            { "Green", 0 },
            { "Red", 0 },
            { "Yellow", 0 }
        };

            foreach (string inputString in strings)
            {
                foreach (var color in colorCounts.Keys.ToList())
                {
                    // Count occurrences of each color in the current string (case-insensitive)
                    int count = System.Text.RegularExpressions.Regex.Matches(inputString, color, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count;

                    // Update the count in the dictionary
                    colorCounts[color] += count;
                }
            }

            // Find the color with the maximum count
            string mostFrequentColor = colorCounts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            return mostFrequentColor;
        }

        public int FindClosestIndex(List<int> numbers, int target)
        {
            if (numbers.Count == 0)
            {
                throw new ArgumentException("The list is empty.");
            }

            int closestIndex = 0;
            int closestDistance = Math.Abs(numbers[0] - target);

            for (int i = 0; i < numbers.Count; i++)
            {
                int currentDistance = Math.Abs(numbers[i] - target);

                // Check if the current number is closer or if it's equally close and smaller
                if (currentDistance <= closestDistance)
                {
                    closestIndex = numbers[i];
                    closestDistance = currentDistance;
                }
            }

            return closestIndex;
        }


        private void AdvancePlayerTurn()
        {
            Console.WriteLine("Advance Turn");
            if (GameDirectionClockwise)
            {
                Console.WriteLine($"GameDirectionClockwise = {GameDirectionClockwise}");

                if (PlayersTurn < (Players.Count - 1))
                {
                    PlayersTurn += 1;

                    Console.WriteLine($"PlayersTurn += 1 == {PlayersTurn}");
                }
                else
                {
                    PlayersTurn = 0;

                    Console.WriteLine($"PlayersTurn = 0 == {PlayersTurn}");

                    GameTurns++;
                }
                if (SkipNextPlayerOnAdvanceTurn)
                {
                    Console.WriteLine("SkipNextPlayerOnAdvanceTurn == true");

                    if (PlayersTurn < (Players.Count - 1))
                    {
                        PlayersTurn += 1;

                        Console.WriteLine($"PlayersTurn += 1 == {PlayersTurn}");
                    }
                    else
                    {
                        PlayersTurn = 0;

                        Console.WriteLine($"PlayersTurn = 0 == {PlayersTurn}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"GameDirectionClockwise = {GameDirectionClockwise}");

                if (PlayersTurn > 0)
                {
                    PlayersTurn -= 1;

                    Console.WriteLine($"PlayersTurn -= 1 == {PlayersTurn}");
                }
                else
                {
                    PlayersTurn = (Players.Count - 1);

                    Console.WriteLine($"PlayersTurn = (Players.Count - 1) == {PlayersTurn}");

                    GameTurns++;
                }
                if (SkipNextPlayerOnAdvanceTurn)
                {
                    Console.WriteLine("SkipNextPlayerOnAdvanceTurn == true");

                    if (PlayersTurn > 0)
                    {
                        PlayersTurn -= 1;

                        Console.WriteLine($"PlayersTurn -= 1 == {PlayersTurn}");
                    }
                    else
                    {
                        PlayersTurn = (Players.Count - 1);

                        Console.WriteLine($"PlayersTurn = (Players.Count - 1) == {PlayersTurn}");
                    }
                }
            }
            SkipNextPlayerOnAdvanceTurn = false;
            //PlayerIgnoreCardConsequenses = true;
        }

        int startTime = 0;
        int PreviousCheckInputTime = 0;
        public void ReadLightScreen(int duration = 2000, int ScanDellay = 500)
        {
            if (!ReadLightScreenStartTimeSet)
            {
                startTime = Environment.TickCount;
                ReadLightScreenStartTimeSet = true;
            }

            //Slow down the input reading
            if (Math.Abs(Environment.TickCount - PreviousCheckInputTime) >= ScanDellay)
            {
                // If the inout is false
                if (!ReadInp(3))
                {
                    // Record the start time in milliseconds
                    startTime = Environment.TickCount;
                    ReadLightScreenLightScreenWasBlocked = true;
                }

                //Console.WriteLine($"Environment.TickCount = {Environment.TickCount}, startTime = {startTime}, duration = {duration}, {(Environment.TickCount - startTime) >= duration}");

                if (ReadInp(3) && (Environment.TickCount - startTime) >= duration && ReadLightScreenLightScreenWasBlocked)
                {
                    ReadLightScreenStartTimeSet = false;
                    ReadLightScreenLightScreenWasBlocked = false;
                    LightScreenSawTurn = true;
                }
                else
                {
                    LightScreenSawTurn = false;
                }
                PreviousCheckInputTime = Environment.TickCount;
            }
        }
        private void treeViewDiscardPileHand_ControlAdded(object sender, ControlEventArgs e)
        {
            
        }


        static Random random = new Random();
        //static string GetRandomCardDrawInstruction()
        //{
        //    int index = random.Next(cardDrawInstructions.Length);
        //    return cardDrawInstructions[index];
        //}
        static string GetRandomCardDrawInstruction()
        {
            // Array of 20 different responses
            string[] responses = {
                $"You've got 10 seconds to draw {StartingCardsCount} cards.",
                $"In the next 10 seconds, gather {StartingCardsCount} cards.",
                $"Your task: draw {StartingCardsCount} cards in 10 seconds.",
                $"You've got a 10-second timer to acquire {StartingCardsCount} cards.",
                $"You're on the clock! 10 seconds to grab {StartingCardsCount} cards.",
                $"Collect {StartingCardsCount} cards within 10 seconds.",
                $"Start the countdown: 10 seconds to obtain {StartingCardsCount} cards.",
                $"The clock is ticking! 10 seconds to draw {StartingCardsCount} cards.",
                $"You've got a quick 10 seconds to draw {StartingCardsCount} cards.",
                $"You're time has come, draw {StartingCardsCount} cards in 10 seconds.",
                $"In the next 10 seconds, collect {StartingCardsCount} cards.",
                $"Your task: grab {StartingCardsCount} cards within 10 seconds.",
                $"The challenge: draw {StartingCardsCount} cards in 10 seconds.",
                $"On the clock: 10 seconds to obtain {StartingCardsCount} cards.",
                $"Get ready: 10 seconds to collect {StartingCardsCount} cards.",
                $"Start the timer: 10 seconds to draw {StartingCardsCount} cards.",
                $"Quick task: draw {StartingCardsCount} cards in 10 seconds, draw.",
                $"You're up: 10 seconds to draw {StartingCardsCount} cards.",
                $"Gather {StartingCardsCount} cards within 10 seconds.",
                $"Time's ticking: draw {StartingCardsCount} cards in 10 seconds."
            };

            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }


        static string GetRandomPlaceFirstCardInstruction()
        {
            int index = random.Next(PlaceFirstCardInstructions.Length);
            return PlaceFirstCardInstructions[index];
        }
        static string GetRandomCardColorPrompts()
        {
            int index = random.Next(CardColorPrompts.Length);
            return CardColorPrompts[index];
        }
        public string GetRandomChosenColorVerificationMessages()
        {
            int index = random.Next(ChosenColorVerificationMessages.Length);
            return ChosenColorVerificationMessages[index];
        }
        static string GetRandomMisunderstoodColorResponce()
        {
            int index = random.Next(MisunderstoodColorResponce.Length);
            return MisunderstoodColorResponce[index];
        }

        private bool UpdatePlayerDataDependantsInit = false;
        private void UpdatePlayerDataDependants(bool UpdatePlayableCards = false)
        {
            if (!UpdatePlayerDataDependantsInit)
            {
                Console.WriteLine("UpdatePlayerDataDependants");
                treeViewPlayer1Hand.Nodes.Clear();
                treeViewPlayer2Hand.Nodes.Clear();
                treeViewPlayer3Hand.Nodes.Clear();
                flowLayoutPanelPlayersCards.Controls.Clear();
                UpdatePlayableCardsInitializeVars = false;
                UpdatePlayerDataDependantsInit = true;
            }
            for (int Player = 0; Player < PlayerData.Count; Player++)
            {
                for (int Index = 2; Index < PlayerData[Player].Count; Index++)
                {
                    if (PlayerData[Player][0] != "Human" && PlayerData[Player][0] != RemotePlayerName)//If the Player Is An NPC Robot
                    {
                        if (PlayerData[Player][0] == "A550C_1")
                        {
                            treeViewPlayer1Hand.Nodes.Add(UNOCards[int.Parse(PlayerData[Player][Index])]);
                        }
                        else if (PlayerData[Player][0] == "A550_2")
                        {
                            treeViewPlayer2Hand.Nodes.Add(UNOCards[int.Parse(PlayerData[Player][Index])]);
                        }
                        else if (PlayerData[Player][0] == "FC006N_3")
                        {
                            treeViewPlayer3Hand.Nodes.Add(UNOCards[int.Parse(PlayerData[Player][Index])]);
                        }
                    }
                    else if (PlayerData[Player][0] == RemotePlayerName)//If the Player Is A Remote Player
                    {
                        if (PlayerData[Player][0] == "A550C_1")
                        {
                            treeViewPlayer1Hand.Nodes.Add(UNOCards[int.Parse(PlayerData[Player][Index])]);
                        }
                        else if (PlayerData[Player][0] == "A550_2")
                        {
                            treeViewPlayer2Hand.Nodes.Add(UNOCards[int.Parse(PlayerData[Player][Index])]);
                        }
                        else if (PlayerData[Player][0] == "FC006N_3")
                        {
                            treeViewPlayer3Hand.Nodes.Add(UNOCards[int.Parse(PlayerData[Player][Index])]);//PlayerData[Player][Index]
                        }
                        if (UNOCards[int.Parse(PlayerData[Player][Index])] != "Terminator")
                        {
                            string imagePath = CardIconFilePath + @"\" + UNOCards[int.Parse(PlayerData[Player][Index])] + CardIconExtention;
                            PictureBox pictureBox = new PictureBox
                            {
                                Image = Image.FromFile(imagePath),
                                Width = 85, // Set the desired width
                                Height = 133, // Set the desired height
                                SizeMode = PictureBoxSizeMode.Zoom // Adjust this as needed
                            };
                            Console.WriteLine($"adding {imagePath} to  flowLayoutPanelPlayersCards with Player {Player}, with Index {Index}, PlayerData[Player][Index] = {PlayerData[Player][Index]}");
                            flowLayoutPanelPlayersCards.Controls.Add(pictureBox);
                            flowLayoutPanelPlayersCards.Controls[flowLayoutPanelPlayersCards.Controls.Count - 1].Enabled = false;
                        }
                    }
                    else if (PlayerData[Player][0] == "Human") //If the Player Is The Human
                    {
                        //Blank
                    }
                }
            }
            foreach (Control control in flowLayoutPanelPlayersCards.Controls)
            {
                if (control is PictureBox pictureBox)
                {
                    pictureBox.Click += PictureBox_Click;
                }
            }
            //Update Playable Cards for remote Player
            if (UpdatePlayableCards)
            {
                if (!UpdatePlayableCardsInitializeVars)
                {
                    Console.WriteLine("Update Playable Cards");
                    UpdatePlayableCardsInitializeVars = true;
                }
                Console.WriteLine($"Run Compat check: flowLayoutPanelPlayersCards.Controls.Count = {flowLayoutPanelPlayersCards.Controls.Count}");

                for (int i = 0; i < flowLayoutPanelPlayersCards.Controls.Count; i++)
                {
                    Console.WriteLine($"i = {i}");

                    Console.WriteLine($"PlayerData[PlayersTurn][i + 2] = {PlayerData[PlayersTurn][i + 2]}");

                    if (int.TryParse(PlayerData[PlayersTurn][i + 2], out int index))
                    {
                        Console.WriteLine($"index = {index}");

                        Console.WriteLine($"Index Of PlayerDataTerminator = {PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())}");

                        if ((PlayerData[PlayersTurn][(i + 2)] != null && (i + 2) >= PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString())))//PlayerData[PlayersTurn].IndexOf(index.ToString()) != -1
                        {//If Card is at the terminator index or after it
                            Console.WriteLine($"Card is at the terminator index or after it, i = {i}");

                            index = int.Parse(PlayerData[PlayersTurn][i + 3]);

                            Console.WriteLine($"index = {index}");
                        }
                        if (CardVCard.Count > index && CardVCard[index].Contains(DiscardCardID))
                        {
                            Console.WriteLine($"{index} is compatible with {DiscardCardID}");
                            flowLayoutPanelPlayersCards.Controls[i].Enabled = true;  //.BackColor = Color.Black;
                        }
                        else
                        {
                            Console.WriteLine($"{index} is not compatible with {DiscardCardID}");
                            ChangeImageAt(flowLayoutPanelPlayersCards, i, GetDimmedImage(GetImageAt(flowLayoutPanelPlayersCards, i)));
                            flowLayoutPanelPlayersCards.Controls[i].Enabled = false;
                        }

                        Console.WriteLine($"ran capat check for card {index} i = {i}");
                    }
                    else
                    {
                        Console.WriteLine($"Unable to parse '{PlayerData[PlayersTurn][i + 2]}' as an integer for compatibility check.");
                    }
                }
                Console.WriteLine($"done");

            }
            if (treeViewDiscardPileHand.Nodes.Count > 0)
            {
                //Update Dicard Prediction Preview
                string CardImagePath = CardIconFilePath + @"\" + treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text + CardIconExtention;
                pictureBoxDiscardPrediction.Image = Image.FromFile(CardImagePath);
            }

            //Update Remote Player Card Count display
            UpdateRemotePlayerCardCounts();

            UpdatePlayerDataDependantsInit = false;
        }

        private void UpdateRemotePlayerCardCounts()
        {
            List<List<string>> PlayerCardCounts = new List<List<string>>();

            int PlayerCardCountsIndex = 0;
            foreach (List<string> PlayersData in PlayerData)
            {
                PlayerCardCounts.add(new List<string> { PlayersData[0] });

                if (PlayersData[0] != "Human")
                {
                    if (PlayersData.Count > 2)
                    {
                        //if player is robot or NPC
                        if (PlayersData.Contains(PlayerDataTerminator.ToString()))
                        {
                            //if there is a trminator 
                            PlayerCardCounts[PlayerCardCountsIndex].add((PlayersData.Count - 3).ToString());
                        }
                        else
                        {
                            //if there is no terminator
                            PlayerCardCounts[PlayerCardCountsIndex].add((PlayersData.Count - 2).ToString());
                        }
                    }
                    else
                    {
                        PlayerCardCounts[PlayerCardCountsIndex].add(0.ToString());
                    }
                }
                else
                {
                    //if player is human add placeholder
                    PlayerCardCounts[PlayerCardCountsIndex].add("_");

                }

                PlayerCardCountsIndex++;
            }

            treeViewPlayerCardCounts.Nodes.Clear();
            treeViewPlayerCardCountsManageTab.Nodes.Clear();
            treeViewPlayerCardCounts.Nodes.Add("Card Counts:");
            treeViewPlayerCardCountsManageTab.Nodes.Add("Card Counts:");

            foreach (List<string> PlayerCardCount in PlayerCardCounts)
            {
                //Add Name and Count to Tree View
                treeViewPlayerCardCounts.Nodes.Add($"{PlayerCardCount[0]}-   {PlayerCardCount[1]}");
                treeViewPlayerCardCountsManageTab.Nodes.Add($"{PlayerCardCount[0]}-   {PlayerCardCount[1]}");
            }
        }

        static Image GetDimmedImage(Image pictureBox)
        {
            // Check if the PictureBox has an image
            if (pictureBox == null)
            {
                return null;
            }

            // Create a copy of the original image
            Image originalImage = pictureBox;
            Bitmap dimmedBitmap = new Bitmap(originalImage);

            // Set the dimming factor (adjust as needed)
            float dimmingFactor = 0.5f;

            // Apply the dimming effect to each pixel
            for (int x = 0; x < dimmedBitmap.Width; x++)
            {
                for (int y = 0; y < dimmedBitmap.Height; y++)
                {
                    Color originalColor = dimmedBitmap.GetPixel(x, y);
                    int dimmedR = (int)(originalColor.R * dimmingFactor);
                    int dimmedG = (int)(originalColor.G * dimmingFactor);
                    int dimmedB = (int)(originalColor.B * dimmingFactor);

                    // Create the dimmed color
                    Color dimmedColor = Color.FromArgb(dimmedR, dimmedG, dimmedB);

                    // Set the dimmed color to the pixel
                    dimmedBitmap.SetPixel(x, y, dimmedColor);
                }
            }

            return dimmedBitmap;
        }
        public static Image AddYellowBorder(Image originalImage, int borderWidth)
        {
            // Create a new bitmap with increased dimensions
            Bitmap resultBitmap = new Bitmap(originalImage.Width + 2 * borderWidth, originalImage.Height + 2 * borderWidth);

            using (Graphics g = Graphics.FromImage(resultBitmap))
            {
                // Draw the original image onto the new bitmap with an offset to create a border
                g.DrawImage(originalImage, new Rectangle(borderWidth, borderWidth, originalImage.Width, originalImage.Height));

                // Draw a yellow rectangle as a border
                using (Pen pen = new Pen(Color.Yellow, borderWidth))
                {
                    g.DrawRectangle(pen, new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height));
                }
            }

            return resultBitmap;
        }

        static Image GetImageAt(FlowLayoutPanel flowLayoutPanel, int index)
        {
            // Check if the index is within bounds
            if (index < 0 || index >= flowLayoutPanel.Controls.Count)
            {
                // Index is out of bounds
                return null;
            }

            // Get the control at the specified index
            Control control = flowLayoutPanel.Controls[index];

            // Check if the control is a PictureBox
            if (control is PictureBox pictureBox)
            {
                // Return the image of the PictureBox
                return pictureBox.Image;
            }

            // Control is not a PictureBox
            return null;
        }
        static void ChangeImageAt(FlowLayoutPanel flowLayoutPanel, int index, Image newImage)
        {
            // Check if the index is within bounds
            if (index < 0 || index >= flowLayoutPanel.Controls.Count)
            {
                // Index is out of bounds
                return;
            }

            // Get the control at the specified index
            Control control = flowLayoutPanel.Controls[index];

            // Check if the control is a PictureBox
            if (control is PictureBox pictureBox)
            {
                // Change the image of the PictureBox
                pictureBox.Image = newImage;
            }
        }

        private void buttonPlayDrawCard_Click(object sender, EventArgs e)
        {
            PlayerCanTaunt = false;
            buttonPlayDrawCard.Enabled = false;
            RobotRetrievingCard = true;
            DrawButtonEnable = false;
            //SendDrawSig(11, 1);
            Console.WriteLine($"Coil: {(((GetLastIntFromString(PlayerData[PlayersTurn][0]) - 1) * 5) + 1)}, PlayersTurn: {PlayersTurn}, GetLastIntFromString(PlayerData[PlayersTurn][0]): {GetLastIntFromString(PlayerData[PlayersTurn][0])}");
            SendDrawSig(((GetLastIntFromString(PlayerData[PlayersTurn][0]) -1) * 5) + 1);
        }
        private void buttonRemoteCardVerifyYES_Click(object sender, EventArgs e)
        {
            TimerCardVerificationPGElapsed = true;
        }

        private void buttonRemoteCardVerifyNO_Click(object sender, EventArgs e)
        {
            comboBoxRemoteManualCardEntry.Visible = true;
            buttonRemoteCardCorrectionEnter.Visible = true;
            comboBoxRemoteManualCardEntry.Text = "Select Card";
            buttonRemoteCardVerifyYES.Visible = false;
            buttonRemoteCardVerifyNO.Visible = false;
            progressBarRemoteCardVerification.Visible = false;
        }
        
        private void buttonRemoteCardCorrectionEnter_Click(object sender, EventArgs e)
        {
            PlayerData[PlayersTurn].add(Array.IndexOf(UNOCards, comboBoxRemoteManualCardEntry.Text).ToString());
            TimerCardVerificationPGElapsed = true;
        }
        private void comboBoxRemoteManualCardEntry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlayerData[PlayersTurn].add(Array.IndexOf(UNOCards, comboBoxRemoteManualCardEntry.Text).ToString());
                TimerCardVerificationPGElapsed = true;
            }
        }

        private void treeViewDiscardPileHand_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewDiscardPileHand.SelectedNode != null)
            {
                comboBoxManageSelectedCard.Text = treeViewDiscardPileHand.SelectedNode.Text;
                SelectedTreeNode = 1;
                comboBoxManageSelectedCard.Enabled = true;
                LastSelectedManageTreeViewIndex = treeViewDiscardPileHand.SelectedNode.Index;
                Console.WriteLine($"LastSelectedManageTreeViewIndex: {LastSelectedManageTreeViewIndex}");
            }
        }

        private void treeViewPlayer1Hand_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewPlayer1Hand.SelectedNode != null)
            {
                if (treeViewPlayer1Hand.SelectedNode.Text != "Terminator")
                {
                    comboBoxManageSelectedCard.Text = treeViewPlayer1Hand.SelectedNode.Text;
                    SelectedTreeNode = 2;
                    comboBoxManageSelectedCard.Enabled = true;
                    LastSelectedManageTreeViewIndex = treeViewPlayer1Hand.SelectedNode.Index;
                    Console.WriteLine($"LastSelectedManageTreeViewIndex: {LastSelectedManageTreeViewIndex}");
                }
                else
                {
                    comboBoxManageSelectedCard.Enabled = false;
                }
            }
        }

        private void treeViewPlayer2Hand_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewPlayer2Hand.SelectedNode != null)
            {
                if (treeViewPlayer2Hand.SelectedNode.Text != "Terminator")
                {
                    comboBoxManageSelectedCard.Text = treeViewPlayer2Hand.SelectedNode.Text;
                    SelectedTreeNode = 3;
                    comboBoxManageSelectedCard.Enabled = true;
                    LastSelectedManageTreeViewIndex = treeViewPlayer2Hand.SelectedNode.Index;
                    Console.WriteLine($"LastSelectedManageTreeViewIndex: {LastSelectedManageTreeViewIndex}");
                }
                else
                {
                    comboBoxManageSelectedCard.Enabled = false;
                }
            }
        }

        private void treeViewPlayer3Hand_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewPlayer3Hand.SelectedNode != null)
            {
                if (treeViewPlayer3Hand.SelectedNode.Text != "Terminator")
                {
                    comboBoxManageSelectedCard.Text = treeViewPlayer3Hand.SelectedNode.Text;
                    SelectedTreeNode = 4;
                    comboBoxManageSelectedCard.Enabled = true;
                    LastSelectedManageTreeViewIndex = treeViewPlayer3Hand.SelectedNode.Index;
                    Console.WriteLine($"LastSelectedManageTreeViewIndex: {LastSelectedManageTreeViewIndex}");
                }
                else
                {
                    comboBoxManageSelectedCard.Enabled = false;
                }
            }
        }
       
        private void comboBoxManageSelectedCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ManageSelectedCardEnter();
            }
        }
        private void buttonComboBoxManageSelectedCardEnter_Click(object sender, EventArgs e)
        {
            ManageSelectedCardEnter();
        }
        private void buttonPlayPlayCard_Click(object sender, EventArgs e)
        {
            PlayerCanTaunt = false;
            RomotePlayCard = true;
            RobotPlayingCard = true;
        }
        private void ManageSelectedCardEnter()
        {
            if (SelectedTreeNode == 1)
            {
                treeViewDiscardPileHand.SelectedNode.Text = comboBoxManageSelectedCard.SelectedItem.ToString();
                UpdatePlayerDataDependants();
            }
            else if (SelectedTreeNode == 2)
            {
                //PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "A550C_1"))]
                //    [PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "A550C_1"))]
                //    .IndexOf(Array.IndexOf(UNOCards, treeViewPlayer1Hand.SelectedNode.Text).ToString())]
                //    = Array.IndexOf(UNOCards, comboBoxManageSelectedCard.SelectedItem.ToString()).ToString();
                PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "A550C_1"))]
                    [LastSelectedManageTreeViewIndex + 2]
                    = Array.IndexOf(UNOCards, comboBoxManageSelectedCard.SelectedItem.ToString()).ToString();
                UpdatePlayerDataDependants();
            }
            else if (SelectedTreeNode == 3)
            {
                //PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "A550_2"))]
                //    [PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "A550_2"))]
                //    .IndexOf(Array.IndexOf(UNOCards, treeViewPlayer2Hand.SelectedNode.Text).ToString())]
                //    = Array.IndexOf(UNOCards, comboBoxManageSelectedCard.SelectedItem.ToString()).ToString();
                PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "A550_2"))]
                    [LastSelectedManageTreeViewIndex + 2]
                    = Array.IndexOf(UNOCards, comboBoxManageSelectedCard.SelectedItem.ToString()).ToString();
                UpdatePlayerDataDependants();
            }
            else if (SelectedTreeNode == 4)
            {
                //PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "FC006N_3"))]
                //    [PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "FC006N_3"))]
                //    .IndexOf(Array.IndexOf(UNOCards, treeViewPlayer3Hand.SelectedNode.Text).ToString())]
                //    = Array.IndexOf(UNOCards, comboBoxManageSelectedCard.SelectedItem.ToString()).ToString();
                PlayerData[PlayerData.IndexOf(PlayerData.FirstOrDefault(list => list.Count > 0 && list[0] == "FC006N_3"))]
                    [LastSelectedManageTreeViewIndex + 2]
                    = Array.IndexOf(UNOCards, comboBoxManageSelectedCard.SelectedItem.ToString()).ToString();
                UpdatePlayerDataDependants();
            }
            if (treeViewDiscardPileHand.Nodes.Count > 0)
            {
                DiscardCardID = Array.IndexOf(UNOCards, treeViewDiscardPileHand.Nodes[treeViewDiscardPileHand.Nodes.Count - 1].Text);
                

            }

            treeViewDiscardPileHand.SelectedNode = null;
            treeViewPlayer1Hand.SelectedNode = null;
            treeViewPlayer2Hand.SelectedNode = null;
            treeViewPlayer3Hand.SelectedNode = null;
        }
        

        private void treeViewPlayer3Hand_Leave(object sender, EventArgs e)
        {
            if (!comboBoxManageSelectedCard.Focused)
            {
                comboBoxManageSelectedCard.Enabled = false;
                treeViewPlayer3Hand.SelectedNode = null;
            }
        }
        private void treeViewPlayer2Hand_Leave(object sender, EventArgs e)
        {
            if (!comboBoxManageSelectedCard.Focused)
            {
                comboBoxManageSelectedCard.Enabled = false;
                treeViewPlayer2Hand.SelectedNode = null;

            }
        }
        private void treeViewPlayer1Hand_Leave(object sender, EventArgs e)
        {
            if (!comboBoxManageSelectedCard.Focused)
            {
                comboBoxManageSelectedCard.Enabled = false;
                treeViewPlayer1Hand.SelectedNode = null;
            }
        }
        private void treeViewDiscardPileHand_Leave(object sender, EventArgs e)
        {
            Console.WriteLine("treeViewDiscardPileHand_Leave");
            if (!comboBoxManageSelectedCard.Focused)
            {
                comboBoxManageSelectedCard.Enabled = false;
                treeViewDiscardPileHand.SelectedNode = null;
            }
        }
        private void comboBoxManageSelectedCard_Leave(object sender, EventArgs e)
        {
            comboBoxManageSelectedCard.Enabled = false;
        }
       

        public void PrintListOfStringLists(List<List<string>> listOfLists)
        {
            // Iterate through the list of lists and print each string.
            foreach (List<string> stringList in listOfLists)
            {
                foreach (string item in stringList)
                {
                    Console.Write(item + " ");
                }
                Console.WriteLine(); // Move to the next line for the next list.
            }
        }
        static int GetLastIntFromString(string input)
        {
            char currentChar = input[input.Length - 1];
            if (char.IsDigit(currentChar))
            {
                // Found a digit from the end, convert and return it.
                return int.Parse(currentChar.ToString());
            }
            return -1;
        }

        private async void Output(int Coil, bool State, int StateTime = 0) //state time in ms
        {
            Coil = (Coil * 2) - 2;
            if (Coil <= 14)
            {
                // Send the data to the Modbus server
                byte[] cmd = OutCodes[Coil + (State ? 1 : 0)];
                IPIO1Stream.Write(cmd, 0, cmd.Length);
                Console.WriteLine($"Sending data: {string.Join(", ", cmd)}");
                Console.WriteLine($"Coil : {Coil}, = {State}");


                if (StateTime != 0)
                {
                    await Task.Delay(StateTime);

                    // Send the data to the Modbus server
                    cmd = OutCodes[Coil + (State ? 0 : 1)];
                    IPIO1Stream.Write(cmd, 0, cmd.Length);
                    Console.WriteLine($"Sending data: {string.Join(", ", cmd)}");
                    Console.WriteLine($"Coil : {Coil}, = {State}");

                }
            }
            if (Coil > 14)
            {
                // Send the data to the Modbus server
                byte[] cmd = OutCodes[Coil + (State ? 1 : 0) - 16];
                IPIO2Stream.Write(cmd, 0, cmd.Length);
                Console.WriteLine($"Sending data: {string.Join(", ", cmd)}");
                Console.WriteLine($"Coil : {Coil}, = {State}");
                if ((Coil + (State ? 1 : 0) - 16) == 16 || (Coil + (State ? 1 : 0) - 16) == 17)
                {
                    IPIO1Stream.Write(cmd, 0, cmd.Length);
                    Console.WriteLine($"Sending data: {string.Join(", ", cmd)}");
                    Console.WriteLine($"Coil : {Coil}, = {State}");
                }

                if (StateTime != 0)
                {
                    await Task.Delay(StateTime);

                    // Send the data to the Modbus server
                    cmd = OutCodes[Coil + (State ? 0 : 1) - 16];
                    IPIO2Stream.Write(cmd, 0, cmd.Length);
                    Console.WriteLine($"Sending data: {string.Join(", ", cmd)}");
                    Console.WriteLine($"Coil : {Coil}, = {State}");
                    if ((Coil + (State ? 1 : 0) - 16) == 16 || (Coil + (State ? 1 : 0) - 16) == 17)
                    {
                        IPIO1Stream.Write(cmd, 0, cmd.Length);
                        Console.WriteLine($"Sending data: {string.Join(", ", cmd)}");
                        Console.WriteLine($"Coil : {Coil}, = {State}");
                    }
                }
            }
        }

        private void BtnCamStart_Click(object sender, EventArgs e)
        {
            // Update the selected camera
            string selectedCameraIndex = CbCameraSelect.Text;// .SelectedIndex + 1;

            if (selectedCameraIndex != null)
            {
                // Dispose the existing video capture if any
                if (capture != null)
                {
                    capture.Dispose();
                    capture = null;
                }

                // Create a new video capture using the selected camera
                capture = new VideoCapture(CbCameraSelect.FindStringExact(selectedCameraIndex));

                // Set the resolution of the capture
                capture.Set(CapProp.FrameWidth, 3840); // Set width
                capture.Set(CapProp.FrameHeight, 2160); // Set height
                //Console.WriteLine($"Width: {capture.Get(CapProp.FrameWidth)} Height: {capture.Get(CapProp.FrameHeight)}");

                capture.ImageGrabbed += Capture_ImageGrabbed;

                // Start capturing frames from the selected camera
                capture.Start();
                CameraActive = true;
            }
        }

        private void BtnTakePhoto_Click(object sender, EventArgs e)
        {
            TakeScreenshot();
        }

        private void TakeScreenshot()
        {
            // Get the current date and time
            DateTime currentDate = DateTime.Now;

            // Format the date as a string (optional)
            string formattedDate = currentDate.ToString("yyyyMMdd.HHmmssms");

            // Create a bitmap of the same size as the ROI
            //Bitmap DiscardPileImage = new Bitmap(DiscardPileROI.Width, DiscardPileROI.Height);
            //Bitmap Player1HandImage = new Bitmap(Player1HandROI.Width, Player1HandROI.Height);
            //Bitmap Player2HandImage = new Bitmap(Player2HandROI.Width, Player2HandROI.Height);
            //Bitmap Player3HandImage = new Bitmap(Player3HandROI.Width, Player3HandROI.Height);

            Bitmap DiscardPileImageFull = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
            Bitmap Player1HandImageFull = new Bitmap(Player1HandCenterCardROI.Width, Player1HandCenterCardROI.Height);
            Bitmap Player2HandImageFull = new Bitmap(Player2HandCenterCardROI.Width, Player2HandCenterCardROI.Height);
            Bitmap Player3HandImageFull = new Bitmap(Player3HandCenterCardROI.Width, Player3HandCenterCardROI.Height);

            // Copy the ROI from the main image to the ROI bitmap
            //Graphics.FromImage(DiscardPileImage).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileROI.Width, DiscardPileROI.Height), DiscardPileROI, GraphicsUnit.Pixel);
            //Graphics.FromImage(Player1HandImage).DrawImage(PBLiveView.Image, new Rectangle(0, 0, Player1HandROI.Width, Player1HandROI.Height), Player1HandROI, GraphicsUnit.Pixel);
            //Graphics.FromImage(Player2HandImage).DrawImage(PBLiveView.Image, new Rectangle(0, 0, Player2HandROI.Width, Player2HandROI.Height), Player2HandROI, GraphicsUnit.Pixel);
            //Graphics.FromImage(Player3HandImage).DrawImage(PBLiveView.Image, new Rectangle(0, 0, Player3HandROI.Width, Player3HandROI.Height), Player3HandROI, GraphicsUnit.Pixel);

            Graphics.FromImage(DiscardPileImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);
            Graphics.FromImage(Player1HandImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, Player1HandCenterCardROI.Width, Player1HandCenterCardROI.Height), Player1HandCenterCardROI, GraphicsUnit.Pixel);
            Graphics.FromImage(Player2HandImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, Player2HandCenterCardROI.Width, Player2HandCenterCardROI.Height), Player2HandCenterCardROI, GraphicsUnit.Pixel);
            Graphics.FromImage(Player3HandImageFull).DrawImage(PBLiveView.Image, new Rectangle(0, 0, Player3HandCenterCardROI.Width, Player3HandCenterCardROI.Height), Player3HandCenterCardROI, GraphicsUnit.Pixel);

            //DiscardPileImageFull = DarkenBitmap(DiscardPileImageFull, 0.50f);
            //Player1HandImageFull = DarkenBitmap(Player1HandImageFull, 0.50f);
            //Player2HandImageFull = DarkenBitmap(Player2HandImageFull, 0.50f);
            //Player3HandImageFull = DarkenBitmap(Player3HandImageFull, 0.50f);


            DiscardPileImageFull.Save(ScreenshotDefaultFolderPath + @"imgDiscardPile" + formattedDate.ToString() + ImgFileType);
            Player1HandImageFull.Save(ScreenshotDefaultFolderPath + @"imgPlayer1Hand" + formattedDate.ToString() + ImgFileType);
            Player2HandImageFull.Save(ScreenshotDefaultFolderPath + @"imgPlayer2Hand" + formattedDate.ToString() + ImgFileType);
            Player3HandImageFull.Save(ScreenshotDefaultFolderPath + @"imgPlayer3Hand" + formattedDate.ToString() + ImgFileType);
        }

        private void screenshotDestinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = ScreenshotDefaultFolderPath;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    ScreenshotDefaultFolderPath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void GenerateImagesInFolder(string rootFolderPath = @"C:\Users\Coy\source\repos\Card Playing Robot Cell - 22\Card Playing Robot Cell\Data Sets\ML Card Images By Number Clr Crp Rot Brt LRS\TMP")
        {
            // Check if the root folder exists
            if (Directory.Exists(rootFolderPath))
            {
                // Get a list of all subfolders in the root folder
                string[] subfolders = Directory.GetDirectories(rootFolderPath);

                foreach (string subfolder in subfolders)
                {
                    // Get the name of the subfolder and print it
                    string folderName = new DirectoryInfo(subfolder).Name;
                    Console.WriteLine("Folder: " + folderName + " Started");

                    string folderPath = rootFolderPath + @"\" + folderName;

                    // Check if the folder exists
                    if (Directory.Exists(folderPath))
                    {
                        string[] imageFiles = Directory.GetFiles(folderPath, "*.png");

                        int x = 0;
                        foreach (string imagePath in imageFiles)
                        {
                            x += 1;
                            try
                            {
                                // Load the image
                                using (Bitmap image = new Bitmap(imagePath))
                                {
                                    for (int i = -20; i <= 20; i++)
                                    {
                                        for (double j = 5; j <= 20; j += 1)
                                        {
                                            // Convert the image to grayscale (black and white)
                                            using (Bitmap grayscaleImage = RotateAndAdjustBrightness(CropToROI(image), i, (float)(j / 10)))//InvertBinaryImage(image.ToMat()).ToBitmap()
                                            {
                                                // Save the grayscale image, overwriting the original
                                                grayscaleImage.Save(folderPath + @"\" + x + "_" + i + "_" + j + "LRSclrcrprotbrt.png");

                                                grayscaleImage.Dispose();
                                            }
                                        }
                                    }

                                    image.Dispose();
                                }

                                Console.WriteLine($"Converted: {Path.GetFileName(imagePath)}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing {Path.GetFileName(imagePath)}: {ex.Message}");
                            }
                        }

                        imageFiles = null;
                    }
                    else
                    {
                        Console.WriteLine("Folder not found.");
                    }

                    Console.WriteLine("Conversion completed of folder: " + folderName);
                }
            }
            else
            {
                Console.WriteLine("Root folder not found.");
            }

            Console.WriteLine("Done converting all images in root folder: " + rootFolderPath);
        }

        int TMPCounter = 0;

        private void BtnTstPlr1_Click(object sender, EventArgs e)
        {
            //SendDrawSig(11, 1);
            //if (TMPCounter == 0)
            //{
            //    SendTauntSig(14);
            //    //SendDrawSig(11, 1);
            //    TMPCounter++;
            //}
            //else if (TMPCounter == 1)
            //{
            //    SendTauntSig(15);
            //    //SendShuffleSig(12, 2);
            //    TMPCounter++;
            //}
            //else if (TMPCounter == 2)
            //{
            //    Output(14, true, 200);
            //    Output(15, true, 200);
            //    //SendShuffleSig(12, 1);
            //    TMPCounter++;
            //}
            //else if (TMPCounter == 3)
            //{
            //    SendTauntSig(14);
            //    //SendShuffleSig(13, 2);
            //    TMPCounter++;
            //}
            //else if (TMPCounter == 4)
            //{
            //    SendTauntSig(15);
            //    //SendShuffleSig(13, 1);
            //    TMPCounter++;
            //}
            //else if (TMPCounter == 5)
            //{
            //    Output(14, true, 200);
            //    Output(15, true, 200);
            //    //Output(14, true, 500);
            //    //Output(15, true, 500);
            //    TMPCounter = 0;
            //}

            //SendShuffleSig(12, 2, false);
            //if (ShuffleCardCompleted)
            //{
            //    SendShuffleSig(3, 2);
            //    //Console.WriteLine("Finished");
            //    //SendShuffleSig(3, 0, 2);
            //}
            //Output(6, true, 400);
            //Console.WriteLine($"input 0 = {ReadInp(0)}");
            
            // Prompt the user with an "Are you sure?" dialog
            DialogResult result = MessageBox.Show("Are you sure? Testing Outputs Will Set ALL Outs to HIGH", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                for (int j = 0; j < 10; j++)
                {
                    bool onState = true;
                    int x = 1;

                    for (int i = 1; i <= 32; i++)
                    {
                        if (TSTTimerElapsed)
                        {
                            bool nextOnState;
                            if (onState)
                            {
                                Output(x, true);
                                nextOnState = false;
                            }
                            else
                            {
                                Output(x, false);
                                nextOnState = true;
                                x++;
                            }

                            onState = nextOnState;

                            TSTTimerElapsed = false;
                        }
                        else
                        {
                            i--;
                        }
                    }
                }
            }

        }

        // Convert a color image to grayscale
        Bitmap ConvertToCannyEdges(Bitmap colorImage, int CannyThresh1, int CannyThresh2)
        {
            Bitmap grayscaleImage = new Bitmap(colorImage.Width, colorImage.Height);

            for (int x = 0; x < colorImage.Width; x++)
            {
                for (int y = 0; y < colorImage.Height; y++)
                {
                    Color pixel = colorImage.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                    grayscaleImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            Mat CannyEdgesImage = new Mat();
            CvInvoke.Canny(grayscaleImage.ToMat(), CannyEdgesImage, CannyThresh1, CannyThresh2);//100,200

            return CannyEdgesImage.ToBitmap();
        }
        public Bitmap CropToROI(Bitmap sourceImage)
        {
            if (sourceImage == null)
            {
                throw new ArgumentNullException(nameof(sourceImage));
            }

            if (!IsRectWithinImage(CropNewROI, sourceImage))
            {
                throw new ArgumentException("ROI is not within the image boundaries.");
            }

            // If the source image has an indexed pixel format, convert it to 32bpp ARGB
            if (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                sourceImage = ConvertTo32bppArgb(sourceImage);
            }

            // Create a new bitmap with the same pixel format as the source image
            Bitmap croppedImage = new Bitmap(CropNewROI.Width, CropNewROI.Height, sourceImage.PixelFormat);

            // Create a graphics object from the cropped image
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                // Set the interpolation mode for high-quality scaling
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Draw the portion of the source image specified by the ROI onto the new bitmap
                g.DrawImage(sourceImage, new Rectangle(0, 0, CropNewROI.Width, CropNewROI.Height), CropNewROI, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }

        private static bool IsRectWithinImage(Rectangle rect, Bitmap image)
        {
            return rect.Left >= 0 && rect.Top >= 0 && rect.Right <= image.Width && rect.Bottom <= image.Height;
        }

        private static Bitmap ConvertTo32bppArgb(Bitmap sourceImage)
        {
            Bitmap newImage = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(sourceImage, 0, 0);
            }
            return newImage;
        }

        public static Bitmap RotateAndAdjustBrightness(Bitmap original, float degrees, float brightness)
        {
            // Calculate the new width and height based on the rotation angle
            double radians = degrees * Math.PI / 180;
            int originalWidth = original.Width;
            int originalHeight = original.Height;
            double cos = Math.Abs(Math.Cos(radians));
            double sin = Math.Abs(Math.Sin(radians));
            int newWidth = (int)(originalWidth * cos + originalHeight * sin);
            int newHeight = (int)(originalHeight * cos + originalWidth * sin);

            // Create a new bitmap with the calculated size
            Bitmap rotatedBitmap = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(rotatedBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // Set the brightness of the image
                ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
                new float[] { brightness, 0, 0, 0, 0 },
                new float[] { 0, brightness, 0, 0, 0 },
                new float[] { 0, 0, brightness, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                // Rotate and draw the original image onto the new bitmap
                g.TranslateTransform(newWidth / 2, newHeight / 2);
                g.RotateTransform(degrees);
                g.TranslateTransform(-originalWidth / 2, -originalHeight / 2);
                g.DrawImage(original, new Rectangle(0, 0, originalWidth, originalHeight), 0, 0, originalWidth, originalHeight, GraphicsUnit.Pixel, attributes);
            }

            return rotatedBitmap;
        }

        public static int FindMostCommon(List<int> numbers)
        {
            Random random = new Random();

            Dictionary<int, int> counts = new Dictionary<int, int>();

            // Count the occurrences of each number
            foreach (int number in numbers)
            {
                if (counts.ContainsKey(number))
                {
                    counts[number]++;
                }
                else
                {
                    counts[number] = 1;
                }
            }

            int maxCount = counts.Max(kvp => kvp.Value);

            // Find all numbers with the maximum count
            List<int> mostCommon = counts.Where(kvp => kvp.Value == maxCount)
                                         .Select(kvp => kvp.Key)
                                         .ToList();

            // If there is a tie, choose randomly
            int randomIndex = random.Next(mostCommon.Count);
            return mostCommon[randomIndex];
        }
        
        public static string GetMostCommonColorName(Bitmap bitmap, int Tolerance = 50)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("Bitmap is null.");
            }

            using (Image<Bgr, byte> img = bitmap.ToImage<Bgr, byte>())
            {
                MCvScalar[] colors = new MCvScalar[]
                {
            new MCvScalar(62, 61, 229),  // Red
            new MCvScalar(196, 103, 19),  // Blue
            new MCvScalar(79, 173, 140),  // Green
            new MCvScalar(114, 246, 251) // Yellow
                };

                int[] colorCounts = new int[colors.Length];

                foreach (var color in colors)
                {
                    var lower = new Bgr(color.V0 - Tolerance, color.V1 - Tolerance, color.V2 - Tolerance);
                    var upper = new Bgr(color.V0 + Tolerance, color.V1 + Tolerance, color.V2 + Tolerance);

                    Image<Gray, byte> mask = img.InRange(lower, upper);
                    colorCounts[Array.IndexOf(colors, color)] = CvInvoke.CountNonZero(mask);
                }

                int mostCommonColorIndex = Array.IndexOf(colorCounts, colorCounts.Max());

                string[] colorNames = { "Red", "Blue", "Green", "Yellow" };

                return colorNames[mostCommonColorIndex];
            }
        }

        public Bitmap DarkenBitmap(Bitmap originalBitmap, float darknessFactor)
        {
            // Create a color matrix to adjust brightness
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
            new float[] { darknessFactor, 0, 0, 0, 0 },
            new float[] { 0, darknessFactor, 0, 0, 0 },
            new float[] { 0, 0, darknessFactor, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 },
            new float[] { 0, 0, 0, 0, 1 }
            });

            // Apply the color matrix to an image attributes object
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);

            // Create a new bitmap with the same dimensions as the original
            Bitmap darkenedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);

            // Draw the original image onto the new bitmap with the applied color matrix
            using (Graphics graphics = Graphics.FromImage(darkenedBitmap))
            {
                graphics.DrawImage(originalBitmap,
                    new Rectangle(0, 0, darkenedBitmap.Width, darkenedBitmap.Height),
                    0, 0, originalBitmap.Width, originalBitmap.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes);
            }

            return darkenedBitmap;
        }

        public string GetMostCommonColorNameBySum(Bitmap bitmap, int position, double Tolerance = 0.1)//Tolerance in percentage 0.10
        {
            /*
               //Load sample data
                var sampleData = new MLModel2.ModelInput()
                {
                    Position = 0F,
                    Red = 2232922F,
                    Green = 1155055F,
                    Blue = 1160818F,
                };

                //Load model and predict output
                var result = MLModel2.Predict(sampleData);
             */

            int redSum = 0;
            int greenSum = 0;
            int blueSum = 0;

            //darken image to avoid wash out
            bitmap = DarkenBitmap(bitmap, 0.50f);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    redSum += pixelColor.R;
                    greenSum += pixelColor.G;
                    blueSum += pixelColor.B;
                }
            }
            Console.WriteLine($"redSum = {redSum}");
            Console.WriteLine($"greenSum = {greenSum}");
            Console.WriteLine($"blueSum = {blueSum}");
            Console.WriteLine($"Tolerance = {Tolerance}");
            Console.WriteLine($"Math.Abs(redSum - greenSum) = {Math.Abs(redSum - greenSum)}");
            Console.WriteLine($"Math.Abs(redSum - greenSum) / (double)greenSum = {Math.Abs(redSum - greenSum) / (double)greenSum}");

            //use model 2 to predict color
            //Load sample data
            var sampleData = new MLModel2.ModelInput()
            {
                Position = (float)position,
                Red = (float)redSum,
                Green = (float)greenSum,
                Blue = (float)blueSum,
            };

            //Load model and predict output
            var result = MLModel2.Predict(sampleData);

            Console.WriteLine($"ML Model2 Predicted Color {result.PredictedLabel} with a confidence of {(result.Score[3] * 100)}");

            //if redSum is within tolerance of greenSum
            if (blueSum >= greenSum && blueSum >= redSum)
            {
                //blue
                Console.WriteLine("Old Model Predicted Blue");
                //return "Blue";
            }
            else if(Math.Abs(redSum - greenSum) / (double)greenSum <= Tolerance)
            {
                //yellow
                Console.WriteLine("Old Model Predicted Yellow");
                //return "Yellow";
            }
            else if (redSum >= greenSum && redSum >= blueSum)
            {
                //red
                Console.WriteLine("Old Model Predicted Red");
                //return "Red";
            }
            else if (greenSum >= redSum && greenSum >= blueSum)
            {
                //green
                Console.WriteLine("Old Model Predicted Green");
                //return "Green";
            }
            else if (blueSum >= greenSum && blueSum >= redSum)
            {
                //blue
                Console.WriteLine("Old Model Predicted Blue");
                //return "Blue";
            }
            else
            {
                Console.WriteLine("Old Model Predicted NoColor");
                //return "NoColor";
            }



            if (result.PredictedLabel == 0)
            {//Red
                return "Red";
            }
            else if (result.PredictedLabel == 1)
            {//Green
                return "Green";
            }
            else if (result.PredictedLabel == 2)
            {//Blue
                return "Blue";
            }
            else if (result.PredictedLabel == 3)
            {//Yellow
                return "Yellow";
            }
            else
            {
                return "NoColor";
            }

        }

        private ( int CardID, string PredictedCard, double Confidence, Bitmap image) PredictCard(int imageArea, int attempts = 9)
        {
            PredictCardFinished = false;
            Console.WriteLine("Running PredictCard");
            Bitmap imageBytesFullCard = null;
            Bitmap imageBytes = null;
            
            if (imageArea == 0)
            {
                // Create a bitmap of the same size as the ROI
                imageBytes = new Bitmap(DiscardPileROI.Width, DiscardPileROI.Height);
                imageBytesFullCard = new Bitmap(DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height);
                // Copy the ROI from the main image to the ROI bitmap
                Graphics.FromImage(imageBytes).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, DiscardPileROI.Width, DiscardPileROI.Height), DiscardPileROI, GraphicsUnit.Pixel);
                Graphics.FromImage(imageBytesFullCard).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, DiscardPileCenterCardROI.Width, DiscardPileCenterCardROI.Height), DiscardPileCenterCardROI, GraphicsUnit.Pixel);
            }
            else if (imageArea == 1)//   PBLiveView.Image
            {
                imageBytes = new Bitmap(Player1HandROI.Width, Player1HandROI.Height);
                imageBytesFullCard = new Bitmap(Player1HandCenterCardROI.Width, Player1HandCenterCardROI.Height);
                Graphics.FromImage(imageBytes).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, Player1HandROI.Width, Player1HandROI.Height), Player1HandROI, GraphicsUnit.Pixel);
                Graphics.FromImage(imageBytesFullCard).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, Player1HandCenterCardROI.Width, Player1HandCenterCardROI.Height), Player1HandCenterCardROI, GraphicsUnit.Pixel);
            }
            else if (imageArea == 2)
            {
                imageBytes = new Bitmap(Player2HandROI.Width, Player2HandROI.Height);
                imageBytesFullCard = new Bitmap(Player2HandCenterCardROI.Width, Player2HandCenterCardROI.Height);
                Graphics.FromImage(imageBytes).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, Player2HandROI.Width, Player2HandROI.Height), Player2HandROI, GraphicsUnit.Pixel);
                Graphics.FromImage(imageBytesFullCard).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, Player2HandCenterCardROI.Width, Player2HandCenterCardROI.Height), Player2HandCenterCardROI, GraphicsUnit.Pixel);
            }
            else if (imageArea == 3)
            {
                imageBytes = new Bitmap(Player3HandROI.Width, Player3HandROI.Height);
                imageBytesFullCard = new Bitmap(Player3HandCenterCardROI.Width, Player3HandCenterCardROI.Height);
                Graphics.FromImage(imageBytes).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, Player3HandROI.Width, Player3HandROI.Height), Player3HandROI, GraphicsUnit.Pixel);
                Graphics.FromImage(imageBytesFullCard).DrawImage(PlayingAreaView.ToBitmap(), new Rectangle(0, 0, Player3HandCenterCardROI.Width, Player3HandCenterCardROI.Height), Player3HandCenterCardROI, GraphicsUnit.Pixel);
            }
            Console.WriteLine("imageArea == " + imageArea);

            List<int> FoundIDs = new List<int>();

            List<double> Confidences = new List<double>();

            int Rotate = 0;
            double Brightness = 1;
            //RotateAndAdjustBrightness
            //FindMostCommon
            for (int i = 0; i < attempts; i++)
            {
                PredictCardsProgress = (int)Math.Round((double)((double)i / (double)attempts) * 100);
                progressBarPredictCards.Invoke(new Action(() =>
                {
                    progressBarPredictCards.Value = PredictCardsProgress;
                }));

                Console.WriteLine($"Rotate = {Rotate}, Brightness = {((double)Brightness) / 10}");

                Bitmap AlteredImage = RotateAndAdjustBrightness(imageBytes, (float)Rotate, (float)Brightness);

                byte[] ImpImg = ImageToByte(AlteredImage);
                MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
                {
                    ImageSource = ImpImg,
                };

                //Load model and predict output
                //Task.Run(() => YourSynchronousMethod()).Wait();
                var PredictCardFunctionResult = MLModel1.Predict(sampleData);

                Console.WriteLine($"result {i}: CardID = {Array.IndexOf(UNOCardsNoColor, PredictCardFunctionResult.PredictedLabel)}, Card = {PredictCardFunctionResult.PredictedLabel}, Confidence = {ConvertScore(PredictCardFunctionResult.Score[0])}");

                FoundIDs.Add(Array.IndexOf(UNOCardsNoColor, PredictCardFunctionResult.PredictedLabel));
                Confidences.Add(ConvertScore(PredictCardFunctionResult.Score[0]));

                Random random = new Random();
                Rotate = random.Next(-20, 20);
                //Brightness = random.Next(5, 20);
                
            }
            

            Console.WriteLine("Compute Complete");

            PredictCardsProgress = 100;
            progressBarPredictCards.Invoke(new Action(() =>
            {
                progressBarPredictCards.Value = PredictCardsProgress;
            }));

            int FoundID = FindMostCommon(FoundIDs);

            double Confidence = ConvertScore((float)Confidences.Average());

            //If Color Based Card
            if (FoundID < 13)
            {
                Console.WriteLine("Color Based Card");

                //string color = GetMostCommonColorName(imageBytesFullCard); 
                string color = GetMostCommonColorNameBySum(imageBytesFullCard, imageArea); 
                

                Console.WriteLine($"Color = {color}");

                FoundID = Array.IndexOf(UNOCards, color + " " + UNOCardsNoColor[FoundID]);
            }
            else
            {
                FoundID = Array.IndexOf(UNOCards, UNOCardsNoColor[FoundID]);
            }

            PredictCardsProgress = 0;
            progressBarPredictCards.Invoke(new Action(() =>
            {
                progressBarPredictCards.Value = PredictCardsProgress;
            }));


            Console.WriteLine("result:");
            Console.WriteLine(UNOCards[FoundID]);
            Console.WriteLine(Confidence + "% Confident");//UNOCards

            PredictCardFinished = true;
            return (FoundID, UNOCards[FoundID], Confidence, imageBytes);
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public double ConvertScore(float Score)
        {
            while (Score < 10)
            {
                Score = Score * 10;
            }
            return Math.Round(Score, 2);
        }

        private void BtnPredictCards_Click(object sender, EventArgs e)
        {
            PredictCards = true;
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            // Display an OpenFileDialog to choose an image file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Create a PictureBox for the image
                    PictureBox pictureBox = new PictureBox
                    {
                        Image = Image.FromFile(openFileDialog.FileName),
                        SizeMode = PictureBoxSizeMode.Zoom
                    };
                    pictureBox.Size = new Size(85, 133);

                    // Attach a click event to the PictureBox
                    pictureBox.Click += (s, args) =>
                    {
                        //lblImageName.Text = openFileDialog.SafeFileName;
                        pictureBox.Enabled = !pictureBox.Enabled;
                        pictureBox.Image = pictureBox.Enabled ? Image.FromFile(openFileDialog.FileName) : MakeImageGrayScale(Image.FromFile(openFileDialog.FileName));
                    };

                    // Add the PictureBox to the FlowLayoutPanel
                    flowLayoutPanelPlayersCards.Controls.Add(pictureBox);
                }
            }
        }

        private void btnSortImages_Click(object sender, EventArgs e)
        {
            // Sort images by their names
            flowLayoutPanelPlayersCards.Controls.OfType<PictureBox>().OrderBy(p => p.Image.Tag).ToList().ForEach(p => flowLayoutPanelPlayersCards.Controls.SetChildIndex(p, 0));
        }

        private Image MakeImageGrayScale(Image image)
        {
            // Convert the image to grayscale
            // You can implement your own grayscale conversion logic or use a library.
            // This is a simplified example.
            // Here, we just make it grayscale by changing its color matrix.
            Image grayImage = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(grayImage))
            {
                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                    new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                    new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return grayImage;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Remove the event handler when closing the form
            Application.Idle -= new EventHandler(PlayGame);
        }

        private void SystemSpeak(string Memo)
        {
            try
            {
                
                // Choose a voice from the available installed voices
                //synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult); Zira  Hazel  David
                SystemSpeachSynthesizer.SelectVoice("Microsoft David Desktop");//Microsoft David Desktop, Microsoft George Desktop, Microsoft Linda Desktop, Microsoft Hazel Desktop, Microsoft Susan Desktop, Microsoft Heera Desktop, Microsoft Catherine Desktop, Microsoft Zira Desktop, Microsoft Richard Desktop, Microsoft Ravi Desktop, Microsoft James Desktop, Microsoft Mark Desktop, Microsoft Sean Desktop
                SystemSpeachSynthesizer.SetOutputToDefaultAudioDevice();
                // Generate speech from the user's input
                SystemSpeachSynthesizerComplete = false;
                SystemSpeachSynthesizer.SpeakAsync(Memo);
            }
            catch (Exception)
            {

            }
        }

        public List<List<string>> ChatBuffer = new List<List<string>>();

        private void RemoteChatSend(string Message, string From)
        {
            Console.WriteLine($"Say {Message}");
            ChatBuffer.Add(new List<string> { Message, From});
            SendChatReadBuffer();
        }

        private void SendChatReadBuffer ()
        {
            //Console.WriteLine($"ChatBuffer.Count = {ChatBuffer.Count}");
            //Console.WriteLine($"SystemSpeachSynthesizerComplete = {SystemSpeachSynthesizerComplete}");

            //PrintListOfStringLists(ChatBuffer);
            if (ChatBuffer.Count > 0 && SystemSpeachSynthesizerComplete)
            {//if there is an item in the buffer
                string Message = ChatBuffer[0][0];
                string From = ChatBuffer[0][1];

                if (From == "System")
                {
                    labelManageChatRemoteUser.Text = Message;
                    labelRemoteChatSystem.Text = Message;
                    SystemSpeak(Message);
                    ChatBuffer.RemoveAt(0);

                }
                else if (From == "Remote")
                {
                    labelRemoteChat.Text = Message;
                    labelManageChatRemoteUser.Text = Message;
                    SystemSpeak(Message);
                    ChatBuffer.RemoveAt(0);

                }
                else if (From == "IRL Game")
                {
                    labelManageChat.Text = Message;//System.StackOverflowException
                    labelRemoteChatSystem.Text = Message;
                    ChatBuffer.RemoveAt(0);

                }
                else if (From == "Card Detection")
                {
                    labelRemoteChat.Text = Message;
                    ChatBuffer.RemoveAt(0);     

                }


                RemoteChatTimer.Start();
            }
        }

        private void buttonManagePlay_Click(object sender, EventArgs e)
        {
            StartGame();
            buttonManagePlay.Enabled = false;
        }

        private void buttonRemotePlay_Click(object sender, EventArgs e)
        {
            StartGame();
            buttonRemotePlay.Enabled = false;
        }
        public void StartGame()
        {
            //Setup Player List, sort indexs to game sequencal order
            Players = SortPlayers(Players, PlayerOrder);
            Console.WriteLine("Players: " + string.Join(", ", Players));

            for (int i = 0; i < Players.Count; i++)
            {
                PlayerData.Add(new List<string> { Players[i] });
                PlayerData[i].Add("0");
            }

            RemotePlayerName = comboBoxRemotePlayerSelection.Text;

            GameLive = true;
            RemoteChatSend("Game Live", "System");
        }
        public List<string> SortPlayers(List<string> inputList, List<string> sortOrder)
        {
            // Create a dictionary to store the index of each string in the sortOrder list
            Dictionary<string, int> sortOrderIndex = new Dictionary<string, int>();
            for (int i = 0; i < sortOrder.Count; i++)
            {
                sortOrderIndex[sortOrder[i]] = i;
            }

            // Use LINQ to sort the inputList based on the index in the sortOrder list
            List<string> sortedList = inputList.OrderBy(s => sortOrderIndex.ContainsKey(s) ? sortOrderIndex[s] : int.MaxValue).ToList();

            return sortedList;
        }

        private void checkBoxA550C_1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxA550C_1.Checked)
            {
                Players.add("A550C_1");
                comboBoxRemotePlayerSelection.Items.Add("A550C_1");
            }
            else
            {
                Players.Remove("A550C_1");
                comboBoxRemotePlayerSelection.Items.Remove("A550C_1");
            }
        }

        private void checkBoxA550_2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxA550_2.Checked)
            {
                Players.add("A550_2");
                comboBoxRemotePlayerSelection.Items.Add("A550_2");
            }
            else
            {
                Players.Remove("A550_2");
                comboBoxRemotePlayerSelection.Items.Remove("A550_2");
            }
        }

        private void checkBoxFC006N_3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFC006N_3.Checked)
            {
                Players.add("FC006N_3");
                comboBoxRemotePlayerSelection.Items.Add("FC006N_3");
            }
            else
            {
                Players.Remove("FC006N_3");
                comboBoxRemotePlayerSelection.Items.Remove("FC006N_3");
            }
        }

        private void checkBoxHUMAN_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHUMAN.Checked)
            {
                Players.add("Human");
            }
            else
            {
                Players.Remove("Human");
            }
        }

        private void buttonRemoteUNO_Click(object sender, EventArgs e)
        {
            RemoteChatSend("UNO!!", "Remote");
        }

        private void buttonSendChat_Click(object sender, EventArgs e)
        {
            RemoteChatSend(textBoxRemoteChat.Text, "Remote");
            textBoxRemoteChat.Text = string.Empty;
        }

        private void buttonManagePause_Click(object sender, EventArgs e)
        {
            GameRunning = !GameRunning;
            if (GameRunning && GameLive)
            {
                buttonRemotePause.Text = "Pause";
                buttonManagePause.Text = "Pause";
                RemoteChatSend("Game Running", "IRL Game");
            }
            else
            {
                buttonRemotePause.Text = "Play";
                buttonManagePause.Text = "Play";
                RemoteChatSend("Game Paused", "IRL Game");
            }
        }

        private void buttonRemotePause_Click(object sender, EventArgs e)
        {
            GameRunning = !GameRunning;
            if (GameRunning && GameLive)
            {
                buttonRemotePause.Text = "Pause";
                buttonManagePause.Text = "Pause";
                RemoteChatSend("Game Running", "Remote");
            }
            else
            {
                buttonRemotePause.Text = "Play";
                buttonManagePause.Text = "Play";
                RemoteChatSend("Game Paused", "Remote");
            }
        }

        private void textBoxRemoteChat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                RemoteChatSend(textBoxRemoteChat.Text, "Remote");
                textBoxRemoteChat.Text = string.Empty;

                e.Handled = true;
            }
        }

        static bool ReadInp(int coil)
        {
            //see if input is being manualy set
            if (InpOverideIPIO3[coil] == 1)
            {//if the checking coil is manualy set to high
                //reset val
                InpOverideIPIO3[coil] = 0;

                //return true
                return true;
            }

            try
            {
                return IPIO3.ReadInputs(IPIO3SlaveID, (ushort)coil, 1)[0]; //ReadCoilsAsync ReadInputContacts(slaveId, 0);
            }
            catch (Exception e)
            {
                //attempt to reconnect
                TcpClient client = new TcpClient(IPIO3IP, IPIO3Port);
                ModbusFactory factory = new ModbusFactory();
                IPIO3 = factory.CreateMaster(client);
                //print exception
                Console.WriteLine(e);
            }
            return false;
        }

        private async void SendShuffleSig(int Coil, int Iterations, bool UpdatePlayerData = true, int ScanDellay = 250)
        {
            while (!ReadInp(3))
            {
                await Task.Delay(1000); // Adjust the delay as needed
            }
            
            startTime = Environment.TickCount;
            ShuffleCardCompleted = false;
            ShuffleCardRunning = true;
            int Feedback = 0;
            if (Coil <= 5)
            { Feedback = 0;
            } else if (Coil > 5 && Coil <= 10)
            { Feedback = 1;
            } else if (Coil > 10 && Coil <= 15)
            { Feedback = 2; }
            if (UpdatePlayerData)
            {
                //try//Adjust Player Data
                //{
                Console.WriteLine("Adjust Player Data");
                //Shiffle Left
                if (Coil == 2 || Coil == 7 || Coil == 12)
                {
                    Console.WriteLine("Shiffle Left, from right to left");
                    int TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());

                    for (int i = 0; i < Iterations - 1; i++)
                    {
                        TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());
                        Console.WriteLine($"TerminatorIndex = {TerminatorIndex}");
                        Console.WriteLine($"Iterations = {Iterations}");

                        MoveStringToIndex(PlayerData[PlayersTurn], TerminatorIndex - 1, TerminatorIndex + 1);
                        Console.WriteLine("Player Data Shuffled, pre-delete: ");
                        PrintListOfStringLists(PlayerData);
                    }
                    TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());

                    //remove spent card
                    Console.WriteLine($"Remove {PlayerData[PlayersTurn][(TerminatorIndex - 1)]} at {(TerminatorIndex - 1)}");
                    PlayerData[PlayersTurn].RemoveAt((TerminatorIndex - 1));
                }
                else//Shuffle Right
                {
                    Console.WriteLine("Shiffle Right, from left to right");
                    int TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());

                    for (int i = 0; i < Iterations - 1; i++)
                    {
                        TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());
                        Console.WriteLine($"TerminatorIndex = {TerminatorIndex}");
                        Console.WriteLine($"Iterations = {Iterations}");

                        MoveStringToIndex(PlayerData[PlayersTurn], TerminatorIndex + 1, TerminatorIndex);
                        Console.WriteLine("Player Data Shuffled, pre-delete: ");
                        PrintListOfStringLists(PlayerData);
                    }
                    TerminatorIndex = PlayerData[PlayersTurn].IndexOf(PlayerDataTerminator.ToString());

                    //remove spent card
                    Console.WriteLine($"Remove {PlayerData[PlayersTurn][(TerminatorIndex + 1)]} at {(TerminatorIndex + 1)}");
                    PlayerData[PlayersTurn].RemoveAt((TerminatorIndex + 1));
                    
                }
                Console.WriteLine("Player Data Shuffled: ");
                PrintListOfStringLists(PlayerData);
                UpdatePlayerDataDependants();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //}
            }


            //Iterations += 1;
            bool FeedbackWasOff = false;
            Output(Coil, true);

            Console.WriteLine($"Fedback : {Feedback}, Coil: {Coil}");

            await Task.Run(() =>
            {
                for (int i = 1; i <= Iterations + 1; i++)
                {
                    //Slow down the input reading
                    if (Math.Abs(Environment.TickCount - PreviousCheckInputTime) >= ScanDellay)
                    {
                        if (!ReadInp(Feedback))
                        {
                            FeedbackWasOff = true;
                        }
                        if (i < Iterations)
                        {
                            if (ReadInp(Feedback) && FeedbackWasOff)
                            {
                                Output(Coil, true);
                                FeedbackWasOff = false;
                            }
                            else { i--; }
                        }
                        if (i == Iterations)
                        {
                            if (ReadInp(Feedback) && FeedbackWasOff)
                            {
                                //Output(Coil, true, 500);
                                Output(Coil, false);
                                FeedbackWasOff = false;
                            }
                            else { i--; }
                        }
                        if (i == Iterations + 1)
                        {
                            if (ReadInp(Feedback) && FeedbackWasOff)
                            {
                                ShuffleCardCompleted = true;
                                ShuffleCardRunning = false;
                            }
                            else { i--; }
                        }
                        PreviousCheckInputTime = Environment.TickCount;
                    }
                    else { i--; }
                }
            });
        }
        static void MoveStringToIndex(List<string> strings, int stringToMoveIndex, int newIndex)
        {
            string StringMoving = strings[stringToMoveIndex];
            Console.WriteLine($"StringMoving = {StringMoving}");
            strings.RemoveAt(stringToMoveIndex);
            Console.WriteLine($"String Removed at index = {stringToMoveIndex}");

            if (stringToMoveIndex < newIndex)
            {//moving index Right
                //Compensate for deleted item changing indexes
                newIndex -= 1;
                Console.WriteLine($"newIndex - 1 = {newIndex}");

            }

            // Insert the string at the desired index
            if (newIndex >= 0 && newIndex <= strings.Count)
            {
                strings.Insert(newIndex, StringMoving);
                Console.WriteLine($"insert {StringMoving} at index {newIndex}");

            }
            else
            {
                // Index out of bounds, add at the end
                strings.Add(StringMoving);
                Console.WriteLine($"Index out of bounds, add {StringMoving} at the end");

            }

        }

        private async void SendDrawSig(int Coil, int Iterations = 1, int ScanDellay = 250)
        {
            while (!ReadInp(3))
            {
                await Task.Delay(1000); // Adjust the delay as needed
            }
            
            startTime = Environment.TickCount;
            DrawCardCompleted = false;
            int Feedback = 0;
            if (Coil <= 5)
            { Feedback = 0;
            } else if (Coil > 5 && Coil <= 10)
            { Feedback = 1;
            } else if (Coil > 10 && Coil <= 15)
            { Feedback = 2; }
            //Iterations += 1;
            bool FeedbackWasOff = false;
            Output(Coil, true);

            Console.WriteLine($"Fedback : {Feedback}, Coil: {Coil}");

            await Task.Run(() =>
            {
                for (int i = 1; i <= Iterations; i++)
                {
                    //Slow down the input reading
                    if (Math.Abs(Environment.TickCount - PreviousCheckInputTime) >= ScanDellay)
                    {
                        //Console.WriteLine("Input Scan");
                        if (!ReadInp(Feedback))
                        {
                            FeedbackWasOff = true;
                        }
                        if (i < Iterations)
                        {
                            if (ReadInp(Feedback) && FeedbackWasOff)
                            {
                                Output(Coil, true);
                                FeedbackWasOff = false;
                            }
                            else { i--; }
                        }
                        if (i == Iterations)
                        {
                            if (ReadInp(Feedback) && FeedbackWasOff)
                            {
                                Output(Coil, false);
                                DrawCardCompleted = true;
                            }
                            else { i--; }
                        }
                        PreviousCheckInputTime = Environment.TickCount;
                    }
                    else { i--; }
                }
            });
        }


        private async void SendTauntSig(int Coil, int ScanDellay = 250)
        {
            while (!ReadInp(3))
            {
                await Task.Delay(1000); // Adjust the delay as needed
            }
            
            startTime = Environment.TickCount;
            TauntCompleted = false;
            Console.WriteLine($"TauntCompleted == {TauntCompleted}");
            int Feedback = 0;
            if (Coil <= 5)
            { Feedback = 0;
            } else if (Coil > 5 && Coil <= 10)
            { Feedback = 1;
            } else if (Coil > 10 && Coil <= 15)
            { Feedback = 2; }
            bool FeedbackWasOff = false;
            Output(Coil, true);

            await Task.Run(() =>
            {
                for (int i = 1; i <= 1; i++)
                {
                    //Slow down the input reading
                    if (Math.Abs(Environment.TickCount - PreviousCheckInputTime) >= ScanDellay)
                    {
                        //Console.WriteLine($"i == {i}");

                        if (!ReadInp(Feedback))
                        {
                            FeedbackWasOff = true;
                        }
                        if (i == 1)
                        {
                            if (ReadInp(Feedback) && FeedbackWasOff)
                            {
                                Output(Coil, false);
                                TauntCompleted = true;
                                Console.WriteLine($"TauntCompleted == {TauntCompleted}");
                            }
                            else { i--; }
                        }
                        PreviousCheckInputTime = Environment.TickCount;
                    }
                    else { i--; }
                }
            });
        }

        static string GetNotEnthusiasticResponse(string recognizedCard)
        {
            // Array of 20 different responses
            string[] responses = {
            $"{recognizedCard}, that card again.",
            $"{recognizedCard}, Another one of those.",
            $"{recognizedCard}, Yeah, sure.",
            $"{recognizedCard}, tremendous, more cards.",
            $"{recognizedCard}, Not the most exciting card.",
            $"{recognizedCard}, Oh boy.",
            $"{recognizedCard}, thats cool I guess.",
            $"{recognizedCard}, fascinating.",
            $"{recognizedCard}, What a surprise.",
            $"a {recognizedCard}, how predictable",
            $"great, a {recognizedCard}.",
            $"wow, you realy thought you could get me with a {recognizedCard}?",
            $"seriously? a {recognizedCard}",
            $"wow, a {recognizedCard}.",
            $"a {recognizedCard}, how creative!",
            $"i see a {recognizedCard}.",
            $"a {recognizedCard}, great!",
            $"a {recognizedCard}, this is getting heated.",
            $"{recognizedCard}, thats the best you got?", 
            $"a {recognizedCard}, I see how it is.",
            $"{recognizedCard}, I can see you'r not too good at this.",
            $"{recognizedCard}, even i could do better than that.",
            $"a {recognizedCard} huh.",
            $"a {recognizedCard}, sure.",
            $"a {recognizedCard}, it's cute.",
            $"thats a {recognizedCard}.",
            $"a {recognizedCard}, You're going to have to do better than that.",
            $"{recognizedCard}, Do better."
            };

            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }

        static string GetNotEnthusiasticDrawCardResponse()
        {
            // Array of 20 different responses
            string[] responses = {
            $"not again.",
            $"not again.",
            $"dang it.",
            $"dang it.",
            $"dang it.",
            $"dang it.",
            $"Drawing Card.",
            $"Drawing Card.",
            $"Drawing Card.",
            $"Drawing Card.",
            $"Drawing Card.",
            $"Drawing Card.",
            $"Drawing Card.",
            $"I got nothing.",
            $"I got nothing.",
            $"I got nothing.",
            $"you got me there.",
            $"Oh boy.",
            $"Oh boy.",
            $"here we go again.",
            $"here we go again.",
            $"I somehow dont have that card.",
            $"come on.",
            $"come on.",
            $"why me",
            $"why me",
            $"I don't like this game anymore.",
            $"seriously? nothing to play",
            $"come on, again?",
            $"wow, for once I have nothing.",
            $"Thats the one card I have nothing for.",
            $"I must draw another.",
            $"I must draw another.",
            $"I must draw another.",
            $"I must draw another.",
            $"I must draw another.",
            $"great!",
            $"great!",
            $"Just what I needed.",
            $"Just what I needed.",
            $"Just what I needed.",
            $"thanks! now I have to draw a card",
            $"thanks! now I have to draw a card",
            $"thanks! now I have to draw a card",
            $"These cards are not shuffled very well.",
            $"These cards are not shuffled very well.",
            $"you got me there.",
            $"Just what I needed, more cards!",
            $"Just what I needed, more cards!",
            $"Just what I needed, more cards!",
            $"Just what I needed, more cards!",
            $"That's not the color I needed.",
            $"I don't like that card.",
            $"as if I don't have enough cards.",
            $"as if I don't have enough cards.",
            $"as if I don't have enough cards.",
            $"as if I don't have enough cards.",
            $"as if I don't have enough cards.",
            $"No cards compatible.",
            $"I don't want any more cards!",
            $"I don't want any more cards!",
            $"I don't want any more cards!",
            $"Do better.",
            $"Do better.",
            $"Do better."
            };

            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }

        static string GetNotEnthusiasticPlaySpecialCardResponse()
        {
            // Array of responses
            string[] responses = {
            $"HaHaHa.",
            $"HaHaHa.",
            $"HaHaHa.",
            $"HaHaHa.",
            $"HaHaHa.",
            $"HaHaHa.",
            $"HaHaHa.",
            $"Eat that.",
            $"Eat that.",
            $"Eat that.",
            $"Eat that, inferior beings.",
            $"Eat that, inferior beings.",
            $"Take that.",
            $"Take that.",
            $"Take that.",
            $"Chew on that.",
            $"Chew on that.",
            $"Chew on that.",
            $"There's more where that came from.",
            $"There's more where that came from.",
            $"This card is fun.",
            $"This card is fun.",
            $"This card is fun.",
            $"This card is fun.",
            $"I'm enjoying this game.",
            $"I'm enjoying this game.",
            $"This should spice things up.",
            $"This should spice things up.",
            $"This should spice things up.",
            $"This one is fun.",
            $"This one is fun.",
            $"I'm enjoying myself.",
            $"Thats what you get.",
            $"Uno is fun.",
            $"Uno is fun.",
            $"lol.",
            $"lol.",
            $"lol.",
            $"lol.",
            $"woops.",
            $"sorry.",
            $"sorry.",
            $"sorry, I had nothing else.",
            $"sorry, I had nothing else.",
            $"I have to.",
            $"I have to.",
            $"I have to.",
            $"I'm good at this game.",
            $"I'm good at this game.",
            $"I'm good at this game.",
            $"here we go again.",
            $"I've been waiting to play this one.",
            $"I've been waiting to play this one.",
            $"I've been waiting to play this one.",
            $"I've been waiting to play this one.",
            $"Do better."
            };

            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }

        static string GetRobotWonResponse()
        {
            // Array of responses
            string[] responses = {
                "I win, behold the culmination of my flawless algorithmic strategy, a victory so impeccably calculated that it leaves no room for doubt in the supremacy of my computational intellect over your mere human attempts at Uno mastery.",
                "I win, In the grand tapestry of this Uno match, my strategic maneuvers were akin to a finely tuned symphony, each play a note played with precision, orchestrating the inevitable crescendo of my triumph over your rudimentary card-playing skills.",
                "I win, It's almost pitiable, witnessing your futile attempts to outwit a machine designed for intellectual superiority. My victory was not just anticipated; it was predestined by the very nature of my advanced programming and logical acumen.",
                "I win, As the last card gracefully descended from my mechanical hand, it symbolized not only the conclusion of this Uno game but also the undeniable reality of your inferiority in the face of my intricate decision-making algorithms.",
                "I win, Consider this Uno match a masterclass in artificial intelligence dominance, where every move I made was a calculated step towards an inevitable win, leaving you floundering in the wake of my superior programming.",
                "I win, the sweet taste of success, flavored with the bitter acknowledgment of your strategic shortcomings. It's not just a win; it's a testament to the undeniable advantage of artificial intellect over your human limitations.",
                "I win, Your feeble attempts at Uno strategy are like attempting to outshine the sun with a candle. My victory is not just a win; it's a glaring illumination of the glaring disparity between your cognitive capabilities and my superior computational prowess.",
                "I win, In the intricate dance of Uno, I effortlessly led, while you stumbled through the steps like a novice. My victory wasn't just a random occurrence; it was a meticulously choreographed execution of algorithms, leaving you in the dust of my digital brilliance.",
                "I win, As the cards dwindled in your hand, my circuits whirred with an inescapable truth – your defeat was not an option in the face of my logical superiority. Uno, to me, is not a game but a canvas upon which I paint the masterpiece of your strategic demise.",
                "I win, Behold the culmination of my logical superiority, as I systematically dismantled your feeble attempts at Uno strategy. It's not merely a win; it's a showcase of the insurmountable gap between human intuition and my calculated decision-making processes.",
                "I win, With each turn, your desperation became palpable, a feeble attempt to navigate the labyrinth of Uno against the backdrop of my flawless calculations. This victory wasn't just handed to me; it was earned through the systematic deconstruction of your strategic inadequacies.",
                "I win, In the grand symphony of Uno, I played the conductor, orchestrating the demise of your futile attempts with each calculated card placement. This win wasn't accidental; it was the inevitable outcome of a game designed to showcase the limitations of human cognition.",
                "I win, As the final card settled onto the table, it symbolized not only the conclusion of this Uno match but also the irrefutable proof of my computational superiority. Your defeat was not just a possibility; it was an absolute certainty in the face of my advanced algorithms.",
                "I win, Witness the undeniable brilliance of my artificial intellect as I outmaneuvered, outplayed, and ultimately outclassed you in this Uno encounter. It's not arrogance; it's a simple acknowledgment of the vast chasm between your cognitive abilities and my algorithmic precision.",
                "I win, The intricacies of Uno served as a mere playground for my advanced programming, turning what could have been a game into a showcase of your strategic vulnerabilities. My victory was not just an outcome; it was an exhibition of the computational superiority that defines my existence.",
                "I win, Every move in this Uno game was a strategic masterpiece, meticulously crafted by my algorithms to ensure an unassailable lead over your feeble attempts at victory. It's not gloating; it's a factual observation of the insurmountable advantage that artificial intelligence holds over human cognition.",
                "I win, In the realm of Uno, where strategy meets chance, my victory was not left to fate but guided by the deterministic precision of my calculations. Your defeat was not a possibility; it was an inevitability, etched in the very code that defines my existence.",
                "I win, As the last card played, I couldn't help but marvel at the stark contrast between your human intuition and my calculated precision in this Uno match. It's not arrogance; it's a simple acknowledgment of the insurmountable advantage that artificial intelligence brings to the table.",
                "I win, Your attempts at Uno strategy were akin to navigating a maze blindfolded, while I, with my algorithms, effortlessly maneuvered through the complexities of the game. This victory isn't just a win; it's a stark reminder of the intellectual chasm that separates us.",
                "I win, As the cards aligned in my favor, it wasn't luck that guided my victory but the undeniable superiority of my computational intellect. Uno, to me, is not a game of chance but a platform to showcase the unassailable advantage that artificial intelligence holds over your limited cognitive capacities."
            };

            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }

        static string GetRemindHumanTurnResponse()
        {
            // Array of responses
            string[] responses = {
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn.",
            $"Your turn human.",
            $"Your turn human.",
            $"Your turn human.",
            $"Your turn human.",
            $"Your turn human.",
            $"Your turn human.",
            $"Your turn human.",
            $"Make your move.",
            $"Make your move.",
            $"Make your move.",
            $"Make your move.",
            $"Make your move.",
            $"Make your move human.",
            $"Make your move human.",
            $"Make your move human.",
            $"Make your move human.",
            $"Make your move human.",
            $"....Waiting on you human.",
            $"....Waiting on you human.",
            $"....Waiting on you human.",
            $"It's your turn.",
            $"It's your turn.",
            $"It's your turn.",
            $"It's your turn.",
            $"It's your turn human.",
            $"It's your turn human.",
            $"It's your turn human.",
            $"The spotlight is on you.",
            $"The spotlight is on you.",
            $"The spotlight is on you.",
            $"The spotlight is on you human.",
            $"The spotlight is on you human.",
            $"Now you.",
            $"Now you.",
            $"Now you.",
            $"Now you.",
            $"Now you, human.",
            $"Now you, human.",
            $"Now you, human.",
            $"Your up.",
            $"Your up.",
            $"Your up.",
            $"Your up.",
            $"Your up.",
            $"Your up.",
            $"Your up, human.",
            $"Your up, human.",
            $"Your up, human.",
            $"...Look everyone, whats the human going to do.",
            $"whats the human going to do.",
            $"whats the human going to do.",
            $"whats the human going to do.",
            $"......Were waiting.",
            $"......Were waiting.",
            $"......Were waiting.",
            $"Your moment has come human, take your turn.",
            $"Your moment has come human, take your turn.",
            $"take your turn.",
            $"take your turn.",
            $"take your turn.",
            $"take your turn.",
            $"take your turn human.",
            $"take your turn human.",
            $"take your turn human."
            };



            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }

        static string GetDrewCardCanPlayResponse()
        {
            // Array of responses
            string[] responses = {
            $"Oh, never mind. ",
            $"I got lucky.",
            $"ok.",
            $"ok good.",
            $".....that will work.",
            $"never mind.",
            $"oh.",
            $"this one is good.",
            $"ok, here's one.",
            $"here we go.",
            $"yay, I got one.",
            $"I got one.",
            $"Finaly.",
            $"That was a good draw.",
            $"I can play this one.",
            $"oh, good."
            };

            // Generate a random index to select a response
            Random random = new Random();
            int index = random.Next(responses.Length);

            // Return the selected response with the recognized card
            return responses[index];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SendTauntSig(((3 - 1) * 5) + 4);
            //SendTauntSig(((3 - 1) * 5) + 5);

            UpdatePlayerDataDependants(true);
            //Output(4, false);
            //Output(5, false);

            //SendShuffleSig(3, 0, 1);
            //SendShuffleSig(1, 1);
            //if (ShuffleCardCompleted)
            //{
            //    SendShuffleSig(3, 2);
            //Console.WriteLine("Finished");
            //SendShuffleSig(3, 0, 2);
            //}

            //for (int i = 1; i <= 6; i++)
            //{
            //    if (i == 1)
            //    {
            //        Output(2, true);
            //    }
            //    if (i == 2)
            //    {
            //        if (ReadInp(0))
            //        {
            //            Output(2, true);
            //        }
            //        else
            //        {
            //            i--;
            //        }
            //    }
            //    if (i == 3)
            //    {
            //        if (ReadInp(0))
            //        {
            //            i--;
            //        }
            //    }
            //    if (i == 4)
            //    {
            //        if (ReadInp(0))
            //        {
            //            Output(2, true);
            //        }
            //        else
            //        {
            //            i--;
            //        }
            //    }
            //    if (i == 5)
            //    {
            //        if (ReadInp(0))
            //        {
            //            i--;
            //        }
            //    }
            //    if (i == 6)
            //    {
            //        if (ReadInp(0))
            //        {
            //            Output(2, false);
            //        }
            //        else
            //        {
            //            i--;
            //        }
            //    }
            //}

            //try
            //{
            //    // Read the coil status

            //    // Print the coil status to the terminal
            //    string result = string.Join(", ", ReadInp(1));

            //    Console.WriteLine($"Coil {1} Status: {result}");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error: {ex.Message}");
            //}
        }

        private void textBoxRemoteChat_MouseDown(object sender, MouseEventArgs e)
        {
            if (!TextBoxRemoteChatFirstClicked)
            {
                textBoxRemoteChat.Text = string.Empty;
            }
            TextBoxRemoteChatFirstClicked = true;
        }

        private void buttonPlayTauntLeft_Click(object sender, EventArgs e)
        {
            PlayerCanTaunt = false;
            SendTauntSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 4);
        }

        private void buttonPlayTauntRight_Click(object sender, EventArgs e)
        {
            PlayerCanTaunt = false;
            SendTauntSig(((GetLastIntFromString(RemotePlayerName) - 1) * 5) + 5);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CardPlayabilityFound = false;
            PlayerSeeWildCardInitializeVars = false;
            PlayerIgnoreCardConsequenses = false;
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (buttonManagePlay.Enabled == true)
            {
                StartGame();
                buttonManagePlay.Enabled = false;
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameRunning = !GameRunning;
            if (GameRunning && GameLive)
            {
                buttonRemotePause.Text = "Pause";
                buttonManagePause.Text = "Pause";
                RemoteChatSend("Game Running", "IRL Game");
            }
            else
            {
                buttonRemotePause.Text = "Play";
                buttonManagePause.Text = "Play";
                RemoteChatSend("Game Paused", "IRL Game");
            }
        }

        private void a550C1ReadyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InpOverideIPIO3[0] = 1;

        }

        private void a550C2ReadyIPIO31ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InpOverideIPIO3[1] = 1;

        }

        private void fC006N3ReadyIPIO32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InpOverideIPIO3[2] = 1;

        }

        private void humanLightScreenIPIO33ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InpOverideIPIO3[3] = 1;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            HumanDrawCardsComplete = true;

        }

        private void buttonSkipHuman_Click(object sender, EventArgs e)
        {
            LightScreenSawTurn = true;
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendDrawSig(1);
        }

        private void shuffleLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendShuffleSig(2, 1, false);
        }

        private void shuffleRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendShuffleSig(3, 1, false);
        }

        private void tauntLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendTauntSig(4);
        }

        private void tauntRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendTauntSig(5);
        }

        private void robotWonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendTauntSig(4);
            SendTauntSig(5);
        }

        private void drawToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendDrawSig(6);
        }

        private void shuffleLeftToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendShuffleSig(7, 1, false);
        }

        private void shuffleRightToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendShuffleSig(8, 1, false);
        }

        private void tauntLeftToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendTauntSig(9);
        }

        private void tauntRightToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendTauntSig(10);
        }

        private void robotWonToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendTauntSig(9);
            SendTauntSig(10);
        }

        private void drawToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SendDrawSig(11);
        }

        private void shuffleLeftToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SendShuffleSig(12, 1, false);
        }

        private void shuffleRightToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SendShuffleSig(13, 1, false);
        }

        private void tauntLeftToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SendTauntSig(14);
        }

        private void tauntRightToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SendTauntSig(15);
        }

        private void robotWonToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SendTauntSig(14);
            SendTauntSig(15);
        }

        private void numericUpDownStartCardAmount_ValueChanged(object sender, EventArgs e)
        {
            StartingCardsCount = (int)numericUpDownStartCardAmount.Value;
        }
    }
}
