# File 内置对象示例
var path = "Cihan.Cli/test/data.txt";
var copyPath = "Cihan.Cli/test/data-copy.txt";
var movedPath = "Cihan.Cli/test/data-moved.txt";

var file = File.open(path);
var original = file.read();

Console.write("文件是否存在: ");
Console.writeln(file.exists());
Console.write("文件大小: ");
Console.writeln(file.size());
Console.write("最后修改时间: ");
Console.writeln(file.lastmodified());
Console.write("原始内容: ");
Console.writeln(original);

file.write("Cihan File API");
Console.write("写入后: ");
Console.writeln(file.read());

file.append(" - Append");
Console.write("追加后: ");
Console.writeln(file.read());

file.copy(copyPath);
Console.write("副本存在: ");
Console.writeln(File.open(copyPath).exists());

File.open(copyPath).move(movedPath);
Console.write("移动后副本存在: ");
Console.writeln(File.open(copyPath).exists());
Console.write("移动后目标存在: ");
Console.writeln(File.open(movedPath).exists());

File.open(movedPath).delete();
file.write(original);
file.close();