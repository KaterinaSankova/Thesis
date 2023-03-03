using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Text.UnicodeEncoding;


Console.OutputEncoding = Encoding.UTF8;
var cmd = new Process
{
    StartInfo =
    {
        FileName = "cmd.exe",
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        UseShellExecute = false
    }
};
cmd.Start();

cmd.StandardInput.WriteLine("chcp 65001");
cmd.StandardInput.Flush();
cmd.StandardInput.Close();

string input = "Reflexivn´ı a tranzitivn´ı bin´arn´ı relace R na X se naz´yv´a kva\u0002ziuspoˇr´ad´an´ı. Antisymetrick´e kvaziuspoˇr´ad´an´ı se naz´yv´a uspoˇr´ad´an´ı. Upln´e uspoˇr´ad´an´ı ´\r\nse naz´yv´a line´arn´ı uspoˇr´ad´an´ı neboli ˇretˇezec. Pokud je R uspoˇr´ad´an´ı na X, pak se\r\nhX, Ri naz´yv´a uspoˇr´adan´a mnoˇzina.!";
var output = new StringBuilder("");
input = input.Replace("´ı", "í");
input = input.Replace("t’", "ť");


Console.WriteLine(input);

int f_index = 0;    //index ve formated
int d_index = 0;

while (input[d_index] != '!')
{
    if (input[d_index] == '´')
    {
        if (input[d_index - 1] == ' ') f_index--;
        d_index++;
        switch (input[d_index])
        {
            case 'a':
                output.Append('á');
                f_index++;
                break;
            case 'e':
                output.Append('é');
                f_index++;
                break;
            case 'y':
                output.Append('ý');
                f_index++;
                break;
            case 'o':
                output.Append('ó');
                f_index++;
                break;
            case 'u':
                output.Append('ú');
                f_index++;
                break;
            case 'A':
                output.Append('Á');
                f_index++;
                break;
            case 'E':
                output.Append('É');
                f_index++;
                break;
            case 'I':
                output.Append('Í');
                f_index++;
                break;
            case 'O':
                output.Append('Ó');
                f_index++;
                break;
            case 'U':
                output.Append('Ú');
                f_index++;
                break;
            default:
                break;
        }
    }
    else if (input[d_index] == 'ˇ')
    {
        if (input[d_index - 1] == ' ') f_index--;
        d_index++;
        switch (input[d_index])
        {
            case 'e':
                output.Append('ě');
                f_index++;
                break;
            case 'z':
                output.Append('ž');
                f_index++;
                break;
            case 'Z':
                output.Append('Ž');
                f_index++;
                break;
            case 's':
                output.Append('š');
                f_index++;
                break;
            case 'S':
                output.Append('Š');
                f_index++;
                break;
            case 'c':
                output.Append('č');
                f_index++;
                break;
            case 'C':
                output.Append('Č');
                f_index++;
                break;
            case 'r':
                output.Append('ř');
                f_index++;
                break;
            case 'R':
                output.Append('Ř');
                f_index++;
                break;
            case 'd':
                output.Append('ď');
                f_index++;
                break;
            case 'D':
                output.Append('Ď');
                f_index++;
                break;
            case 't':
                output.Append('ť');
                f_index++;
                break;
            case 'T':
                output.Append('Ť');
                f_index++;
                break;
            case 'n':
                output.Append('ň');
                f_index++;
                break;
            case 'N':
                output.Append('Ň');
                f_index++;
                break;
            default:
                break;
        }
    }
    else if (input[d_index] == '°')
    {
        if (input[d_index - 1] == ' ') f_index--;
        d_index++;
        output.Append('ů');
        f_index++;
    }
    else
    {
        output.Append(input[d_index]);
        f_index++;
    }
    d_index++;
}

foreach (var x in new List<char> {'x', 'y', 'z', 'X', 'R'})
{
    output.Replace($"h{x}", $"<{x}");
    output.Replace($"{x}i", $"{x}>");
}
foreach (var x in new List<char> { '1', '2', '3' })
{
    output.Replace($"{x}i", $"{x}>");
}

output.Replace("ex>", "exi");
output.Replace("˚u", "ů");

output.Replace("6=", "≠");
output.Replace("6∈", "∉");

await File.WriteAllTextAsync("WriteText.txt", output.ToString());
