# `generate` 指令

**介绍**：`generate` 指令用于使用 API 出一个数独题目。

**用法**：`generate --hard-pattern (--count <范围>)?`

## 可选参数

### `--count` 参数（简写 `-c`）

`--count` 参数用来表示出题的时候，提示数个数的限制范围。该范围用范围记号 `..` 表示。比如 `24..30` 表示出的题目的提示数总个数只要在 24 个到 29 个之间的情况的话，题目才叫满足条件。

范围记号的两侧的数字可以省略。如果省略左侧的数字（即 `..最大值`），则默认为 17（因为唯一解的题目最少有 17 个提示数）；而如果省略右侧的数字（即 `最小值..`），则默认为 81（因为盘面至少有一个空格）；两侧都省略（即 `..`），则等价被视为 `17..81`。范围记号的左右两侧不能有空格，这是因为控制台程序匹配的时候的处理规则要求不能包含空格，空格用于分割参数。

## 组参数

### `--hard-pattern` 参数（简写 `-h`）

`--hard-pattern` 参数用来表示出题使用难题模型来进行出题。

另外请注意，由于算法的限制，使用该参数的出题器只能保证题目的出题模型满足难题的模型，但不代表题目一定就很难，只不过，使用该参数出的题目一般都比普通的出题规则出的题要难一些。