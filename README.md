# Introduction
MCSharp (or MC#, or MC-Sharp) is a scripting language that lets you compile down high-level code to datapacks that you can load in Minecraft. It supports many modern language features, including: 
* Control Structures (if/else, for, while etc)
* Preprocessor directives
* Namespaces

Apart from the pure coding, MCSharp also allows you to implement other aspects of datapacks more easily, like defining strcutres by formulating a palette of blocks and then "drawing" the building as plain text.

# The language

## Keywords
Keywords are words that have special meaning in MCSharp and thus cannot be used as names for functions, variables or other parts of your code that requires identifiers. Preprocessor directives are not considered keywords as they have special syntax. MCSharp contains the following keywords:

`and, const, else, float, for, if, int, is, namespace, not, or, return, unless, using, while`

## Preprocessor Directives
Preprocessor directives are not actual code that the program executes. Instead, they are instructions for the preprocessor, the piece of the compiler that analyzes and cleans up the source code for further processing. Preprocessor directives allow you to modify the behaviour of the preprocessor, telling it if it should ignore certain pieces of code or replace one string of characters with another one. MCSharp supports the following preprocessor directives

`#define, #elif, #else, #endif, #endversion, #ifdef, #undef, #version`

## Comments
Comments allow you to, well, comment your code and explain aspects of your program that may not be obvious. They have no special syntax apart from characters that begin the comment. Comments are ignored completely by the compiler, with the preprocessor discarding them immediately

`//` starts a single-line comment. Ignored in strings, terminated by a newline.
`/*` and `*/` start and terminate a block comment, with the actual comment nbetween them. Ignored in strings, new lines do not terminate the comment.

