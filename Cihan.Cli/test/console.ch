# Console 内置对象示例
Console.clear();
Console.setcursorposition(0, 0);

Console.write("姓名: ");
var name = Console.readln();
Console.writeln("你好，" + name + "！");

Console.write("按任意键继续: ");
var key = Console.readkey();
Console.writeln();
Console.writeln("你按下了: " + key);
