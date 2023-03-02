using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Drawing;
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

Stream demotivate(string text, string photoName)
{
    Image photo = 
    using Stream stream = System.IO.File.OpenRead("./photo.jpg");
    return stream;
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    Message sendMessage = await botClient.SendPhotoAsync(
        chatId: message.Chat.Id,
        photo: new InputFile(content:stream, fileName: "photo.jpg"),
        cancellationToken: cancellationToken);
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