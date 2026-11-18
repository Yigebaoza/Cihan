var s = ""; // 声明变量

while (True) {
    s = Console.readln("请输入: "); // 等同于Python: s = input("请键入")
    if (s == "hello") {
        Console.writeln("Hi");
    }
    else if (s == "hi") {
        Console.writeln("Hello");
    }
    else if (s == "goodbye" || s == "bye-bye") {
        break;
    }
    else if (s == "") {
        continue;
    }
    else {
        Console.writeln("你输入的是: " + s);
    }
}