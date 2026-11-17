namespace Cihan.Core.Lexing;

/// <summary>
/// 词法单元类型。
/// </summary>
public enum TokenKind
{
	// ===== 字面量 =====
	StringLiteral,   // 字符串
	NumberLiteral,   // 数字
	True,            // 布尔 true
	False,           // 布尔 false

	// ===== 关键字 =====
	Var,             // var
	Const,           // const
	If,              // if
	Else,            // else
	While,           // while
	Break,           // break
	Continue,        // continue
	Func,            // func
	Return,          // return

	// ===== 标点 =====
	Dot,             // .
	LeftParen,       // (
	RightParen,      // )
	Comma,           // ,
	Semicolon,       // ;
	LeftBrace,       // {
	RightBrace,      // }

	// ===== 运算符 =====
	Plus,            // +
	Minus,           // -
	Star,            // *
	Slash,           // /
	Equals,          // =  赋值
	EqualEqual,      // == 相等比较
	BangEquals,      // != 不等比较
	Less,            // <
	Greater,         // >
	LessOrEqual,     // <=
	GreaterOrEqual,  // >=

	AmpAmp,          // &&
	PipePipe,        // ||
	Bang,            // !
	PlusPlus,        // ++
	MinusMinus,      // --
					 // ===== 特殊 =====
	Identifier,      // 标识符
	Eof,             // End Of File，输入结束标记
}