# Cihan

Cihan 是一个使用 C# 实现的轻量级解释型脚本语言。项目包含词法分析、语法解析、AST 求值，以及 `Console` 和 `File` 内置对象。

## 项目结构

```text
Cihan.sln
Cihan.Core/         词法分析、AST 和语法解析
Cihan.Runtime/      求值器与内置对象
Cihan.Cli/          命令行入口和示例脚本
README.md
```

## 环境要求

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## 构建和运行

在根目录构建：

```powershell
dotnet build Cihan.sln
```

运行脚本：

```powershell
dotnet run --project Cihan.Cli -- <脚本路径>
```

例如：

```powershell
dotnet run --project Cihan.Cli -- Cihan.Cli/test/helloworld.ch
dotnet run --project Cihan.Cli -- Cihan.Cli/test/while_for.ch
dotnet run --project Cihan.Cli -- Cihan.Cli/test/func.ch
dotnet run --project Cihan.Cli -- Cihan.Cli/test/file.ch
```

调试选项：

```powershell
dotnet run --project Cihan.Cli -- Cihan.Cli/test/helloworld.ch -showTks
dotnet run --project Cihan.Cli -- Cihan.Cli/test/helloworld.ch -showStms
```

- `-showTks`：输出词法分析结果
- `-showStms`：输出解析后的语句

脚本通常使用 `.ch` 或 `.cihan` 后缀。

## 快速开始

创建 `hello.ch`：

```csharp
var name = "Cihan";
Console.writeln("Hello, " + name + "!");
```

运行：

```powershell
dotnet run --project Cihan.Cli -- hello.ch
```

输出：

```text
Hello, Cihan!
```

## 语言语法

### 注释

使用 `#` 编写单行注释：

```csharp
# 这是一条注释
var value = 1;
```

### 变量和常量

```csharp
var count = 0;
const name = "Cihan";

count = count + 1;
del count;
```

- `var` 声明变量
- `const` 声明常量，不能重新赋值
- `del` 删除当前作用域中已声明的变量或函数
- 变量声明、赋值、`del`、`break`、`continue` 和 `return` 通常需要分号

### 数据类型

- 数字：`42`、`3.14`
- 字符串：`"Hello"`
- 布尔值：`True`、`False`
- `null`：函数无返回值或 `return` 无表达式时产生

### 运算符

```text
算术：+  -  *  /  ++  --  -
比较：==  !=  <  >  <=  >=
逻辑：&&  ||  !
```

字符串只支持字符串与字符串相加和比较。布尔值参与数字运算时会转换为 `1` 或 `0`。

### 条件分支

```csharp
var score = 90;

if (score >= 90) {
    Console.writeln("优秀");
}
else if (score >= 60) {
    Console.writeln("及格");
}
else {
    Console.writeln("不及格");
}
```

### while 循环

```csharp
var i = 0;

while (i < 5) {
    Console.writeln(i);
    i++;
}

del i;
```

循环中可使用 `break` 和 `continue`。

### for 循环

```csharp
for (var i = 0; i < 5; i++) {
    Console.writeln(i);
}
```

## 函数

使用 `func` 声明函数；参数支持默认值。

```csharp
func hello(name, num = 2) {
    for (var i = 0; i < num; i++) {
        Console.write("Hello, ");
        Console.writeln(name);
    }
}

func add(a, b) {
    return a + b;
}

hello("Amy");
hello("Bob", 3);
Console.writeln(add(1, 2));
```

函数调用时会创建独立环境，函数可以捕获声明时的作用域并向父作用域查找变量。

## 内置对象

### Console

| 方法 | 参数 | 说明 |
| --- | --- | --- |
| `Console.write(value)` | 可选 | 不换行输出 |
| `Console.writeln(value)` | 可选 | 换行输出 |
| `Console.readln(prompt)` | 可选 | 输出提示并读取一行 |
| `Console.readkey(prompt)` | 可选 | 输出提示并读取一个字符 |
| `Console.clear()` | 无 | 清空控制台 |
| `Console.setcursorposition(left, top)` | 两个整数 | 设置光标位置 |

示例：

```csharp
Console.clear();
Console.setcursorposition(0, 0);

var name = Console.readln("请输入名字: ");
Console.writeln("你好，" + name);

Console.write("按任意键继续: ");
var key = Console.readkey();
Console.writeln(key);
```

完整示例见 [console.ch](Cihan.Cli/test/console.ch)。

### File

| 方法 | 参数 | 说明 |
| --- | --- | --- |
| `File.open(path)` | 路径 | 创建文件句柄 |
| `file.read()` | 无 | 读取全部文本 |
| `file.read(length)` | 字符数 | 从文件读取指定长度 |
| `file.write(content)` | 文本 | 覆盖写入文件 |
| `file.append(content)` | 文本 | 在文件末尾追加 |
| `file.delete()` | 无 | 删除文件 |
| `file.exists()` | 无 | 判断文件是否存在 |
| `file.size()` | 无 | 获取文件字节数 |
| `file.lastmodified()` | 无 | 获取最后修改时间 |
| `file.copy(destinationPath)` | 目标路径 | 复制文件 |
| `file.move(destinationPath)` | 目标路径 | 移动文件 |
| `file.close()` | 无 | 当前实现中不持有文件资源 |

示例：

```csharp
var file = File.open("Cihan.Cli/test/data.txt");

Console.writeln(file.read());
file.append("!!");
Console.writeln(file.read());
Console.writeln(file.exists());

file.close();
```

完整示例见 [file.ch](Cihan.Cli/test/file.ch)。示例脚本会复制、移动和删除临时文件，并恢复 `data.txt` 的原始内容。

## 作用域

- 全局作用域保存全局变量
- 普通代码块拥有独立子作用域
- `for` 循环拥有独立作用域
- 函数调用时创建独立环境
- 函数可以捕获声明时的作用域

## 示例脚本

- [helloworld.ch](Cihan.Cli/test/helloworld.ch)：变量和控制台输出
- [while_for.ch](Cihan.Cli/test/while_for.ch)：`while`、`for`、`break` 和 `continue`
- [func.ch](Cihan.Cli/test/func.ch)：函数、默认参数和返回值
- [input.ch](Cihan.Cli/test/input.ch)：控制台输入和条件分支
- [console.ch](Cihan.Cli/test/console.ch)：`Console` API 示例
- [file.ch](Cihan.Cli/test/file.ch)：`File` API 示例
