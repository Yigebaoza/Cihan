# 我的分号怎么没用了???

func hello(name, num = 2) {
    for (var i = 0; i < num; i ++) {
        Console.write("Hello, ");
        Console.writeln(name);
    }
}

func add(a, b) {
    return a + b;
}

hello("Jhon");
hello("Amy", 3);

hello(add(1, 1), 1);