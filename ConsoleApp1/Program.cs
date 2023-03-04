using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Net.Mime;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using Color = Telegram.Bot.Types.Color;
using File = Telegram.Bot.Types.File;

StreamReader f = System.IO.File.OpenText("./token");
var token = f.ReadLine();

var botClient = new TelegramBotClient(token);

using CancellationTokenSource cts = new ();

ReceiverOptions receiverOptions = new ()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();
System.Console.ReadLine();
cts.Cancel();

void demotivate_Text(string text, string save)
{
    Image photo = Image.Load(save);
    int size;
    float padding;
    if (text.Length < 25)
    {
        size = 50;
        padding = (float)(size / 3.8 * text.Length);
    }
    else
    {
        size = 30;
        padding = (float)(size / 7.9 * text.Length);
        var sub = text.Split().ToList();
        if (sub.Count == 1)
        {
            sub[0] = text.Substring(0, (int)text.Length / 2);
            sub.Add(text.Substring((int)text.Length/2));
        }
        sub[(int)sub.Count / 2 - 1] += "\n";
        text = String.Join(" ", sub.Select(x => x.ToString()).ToArray());
    }
    System.Console.WriteLine(text);

    Font font = SystemFonts.CreateFont("Arial", size, FontStyle.Regular);
    photo.Mutate(x => x.DrawText(text, font, SixLabors.ImageSharp.Color.White, new PointF((float) (photo.Width / 2 - padding), photo.Height / 2 + 130 )));
    photo.Save(save);
    //using Stream stream = System.IO.File.OpenRead("./photo.jpg");
    //return stream;
    
}

void demotivate_Photo(string photoName, string save)
{
    Image<Rgba64> photo = new(640, 480); 
    Image image = Image.Load(photoName);
    image.Mutate(x => x.Resize(400, 300));
    
    var y_pos = photo.Height / 2 - 200;
    var x_pos = photo.Width / 2 - 200;
    Rectangle rect_w = new Rectangle(x_pos - 8, y_pos - 8, 416, 316);
    Rectangle rect_b = new Rectangle(x_pos - 5, y_pos - 5, 410, 310);
    photo.Mutate(x => x.Fill(SixLabors.ImageSharp.Color.White, rect_w));
    photo.Mutate(x => x.Fill(SixLabors.ImageSharp.Color.Black, rect_b));
    
    photo.Mutate(x => x.DrawImage(image, new Point(x_pos, y_pos), 1));

    photo.Save(save);
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    var image = "./" + message.Chat.Id + "i.jpg";
    var photo = "./" + message.Chat.Id + "p.jpg"; 
    if (update.Message.Type == MessageType.Photo)
    {
        string destinationFilePath = image;

        await using Stream fileStream = System.IO.File.OpenWrite(destinationFilePath); 
        var fileId = update.Message.Photo.Last().FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;
        await botClient.DownloadFileAsync(
            filePath: filePath,
            destination: fileStream,
            cancellationToken: cancellationToken);
        fileStream.Close();
        demotivate_Photo(destinationFilePath, photo);
    }

    if (update.Message.Type == MessageType.Text)
    {
        if (System.IO.File.Exists(photo) == false)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Send photo first",
                cancellationToken: cancellationToken
            );
            return;
        }

        
        demotivate_Text(update.Message.Text, photo);
        var mes = System.IO.File.OpenRead(photo);
        Message sendMessage = await botClient.SendPhotoAsync(
            chatId: message.Chat.Id,
            photo: new InputFile(content:mes, fileName: photo),
            cancellationToken: cancellationToken);
        mes.Close();
        System.IO.File.Delete(photo);
        System.IO.File.Delete(image);
    }

}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}