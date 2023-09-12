# Introduction
MCSharp (or MC#, or MC-Sharp) is a scripting language that lets you compile down high-level code to datapacks that you can load in Minecraft. It supports a few modern language features, including: 
* Control Structures (if/else, for, while etc)
* Object Oriented programming for entities
* Namespaces

Apart from the pure coding, MCSharp also allows you to implement other aspects of datapacks more easily, like defining strcutres by formulating a palette of blocks and then "drawing" the building as plain text or bitmaps.

# The language

## Keywords
Keywords are words that have special meaning in MCSharp and thus cannot be used as names for functions, variables or other parts of your code that requires identifiers. MCSharp contains the following keywords:

`and, const, else, float, for, if, int, is, namespace, not, or, return, unless, while`

## Primitive data types
Primitive data types are the built-in data types that already exist within the definition of a programming language and usually those that CPUs already work with on a basic level. Since this languages target "CPU" will be the game of Minecraft, we will be working with the following data types:

`byte, boolean, double, float, int, long, short, string, vector3, vector3i`

`byte, short, int` and `long` are all signed intergers with a size of 8, 16, 32 and 64 bits respectively. `boolean` are true/false values. Their theoretical size is 1 bit, although the real size may be larger, at least 8 bits. `float` and `double` are 32 bit and 64 bit signed floating point numbers, meaning the digits before and after the decimal point is not fixed. `string` is a sequence of characters. `vector3` and `vector3i` are 3 dimensional floating point and integer vectors. Their components are of type `float` and `int` and have the same restraints as the respective scalar types.

## Comments
Comments allow you to, well, comment your code and explain aspects of your program that may not be obvious. They have no special syntax apart from characters that begin the comment. Comments are ignored completely by the compiler, with the preprocessor discarding them immediately. However, a command line option allows you to keep comments in the compiled files.

`//` starts a single-line comment. Ignored in strings, terminated by a newline.
`/*` and `*/` start and terminate a block comment, with the actual comment inbetween them. Ignored in strings, new lines do not terminate the comment.

# The Logging system

The logging system aims to only print relevant messages to the user. Each message has a certain Log Level which itself is associated with a signed 32-bit integer called its "verbosity rating". Larger values are more verbose and smaller values are less verbose. The actual verbosity ratings are chosen arbitrarily, with 0 being considered default. Running the compiler with the command line argument `-L <value>` will set the verbosity level of the compilation process to whatever value was passed. Any log messages with a verbosity rating equal or less than the set verbosity level will be printed. Current verbosity levels and ratings are:
| Verbosity | Rating |
| :-: | :-: |
| Debug | 2147483647<br/>(maximum 32-bit integer) |
| Hint | 1 |
| Info | -10 |
| Warn | -100 |
| Error | -2147483648<br/>(minimum 32-bit integer) |

Note 1: This means no matter how low you set the chosen verbosity level, errors will always be printed as they will always be either equal or less than the chosen level. This is by design, as errors are messages that prevent a program from compiling correctly and give insight into where and what exactly prevents the compiler from acomplishing its task.

Note 2: That Debug messages are only available with the highest possible verbosity level. This is also by design, as debug messages are only intended for the developer and hold no real value in an end user scenario.

Note 3: Some Log messages overrule the passed settings so that they can always display regardless of selected verbosity and still have their appropriate log level. Otherwise, messages that would have to be displayed would have to use `Error` as their log level, even if they weren't errors at all.