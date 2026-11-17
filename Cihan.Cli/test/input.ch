var s = "";

while (True) {
    s = Console.readln("请输入: ");
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