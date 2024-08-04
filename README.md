# AtelierTomato.Dice

# Guide
This guide will offer a basic overview on how to set up implementations of AtelierTomato.Calculator or AtelierTomato.Dice.

## Calculator
A calculator needs an instance of two classes: `ExpressionParser` and `ExpressionExecutor`.
Steps:
- Run the parser `ExpressionParser.Parse()`, the expected input is a mathematical equation that may use parentheses, exponents, multiplication/division, and addition/subtraction.
- Run the executor `ExpressionExecutor.Calculate()` with the output of the parser, which will return a double value.

## Dice
A die needs an instance of two classes: `DiceExpressionParser` and `DiceExpressionExecutor`.
 Steps:
- Run the parser `DiceExpressionParser.Parse()`, the expected input is anything from the stock `ExpressionParser` as well as anything represtenting a die or options for a die, more on this below.
- Run the executor `DiceExpressionExecutor.Calculate()` with the output of the parser, which will return a double value.

## Dice explanation
Basic rolls follow a `#d#` setup where the first number is the amount of dice and the second is the amount of sides. All calculations happen last, unless they are in parentheses. The terminology used to refer to aspects of the dice roll is complex, so here is an example of what each part is:
In the dice parsed from the string `1d20+10d20e1`, `1d20` and `10d20e1` are the dice expressions. Dice expressions inform the program what kind of dice to roll (the number after the `d`) and how often (number before the `d`). Optionally, parameters after the second number may be added for more complex dice roll scenarios.
Multiple complex dice can be rolled and added together or whatever you prefer in a single command, all parameters that do not directly conflict with each other can be used at the same time. The different extra parameters are as follows. All # signs refer to any number, anything in parentheses is optional to the parameter.
### o
[O]rders a dice expression's rolls by descending size.
### (i)r#(;#)
[R]erolls the die until it rolls at least the first number. The second number determines how many times the dice will be rerolled, if no second number is hit, the default value set in the options will be used. If the preceding `i` is present, the die will be rerolled [i]nfinitely until they hit this number.
### (i)e#(;#)
[E]xplodes the die if it hits the first number. Explosion means that it will roll the same die again, and then add on the resulting roll to the total value for that individual roll. The second number determines how many times the die can explode, this does not include the initial roll, only explosion rolls after the fact (so a `1d20e20;10` has a maximum value of `220`). If no second number is set, the default value set in the options will be used. The preceding `i` will allow the die to be rerolled [i]nfinitely.
### p#
Dro[p]s the set number of dice rolled which are of the lowest values.
### k#
[K]eeps the set number of dice which rolled the highest values, meaning the result will only include the amount of dice set.
### t# and f#
[T]arget and [f]ailure, when set, will turn the resulting number of a dice expression from the sum of the dice rolled to a different kind of roll where any roll at or above the [t]arget number will add one to the result and any roll at or below the [f]ailure number will subtract one from the result.
### s and h
[S]how and [h]ide determine whether or not this dice expression will output in the message or not, with [h]idden expressions not outputting any text and [s]hown expressions outputting the total of the dice rolls and optionally individual dice. Uses the default set in the options if not specified.
### q and v
[Q]uiet and [v]erbose determine if each individual roll in a dice expression will be shown or not, with [q]uiet dice expressions only showing the result and [v]erbose dice expressions showing each individual roll. Uses the default set in the options if not specified.
