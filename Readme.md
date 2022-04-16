﻿# LineAdjustment

## Входящие данные
- `string` произвольный текст
- `int` ширина строки в символах

## Исходящие данные
- `string` преобразованный текст

## Описание преобразования
- Входящая строка делится на условные слова по пробелам. 
- Извлечённые слова объединяются в строки с учётом ограничения строки по ширине. Строка должна включать максимальное количество слов. Внутри строки слова растянуты между собою по ширине строки:
    - Расстояние между словами заполняется равным количеством пробелов.
    - Если количество пробелов равным сделать невозможно - то добавляем
еще по пробелу между словами слева направо.
    - Если в строке помещается только 1 слово, то дополнить строку пробелами справа.
- Результирующий текст состоит из собранных строк, разделённых между собою символом переноса строки.

## Оригинальное описание
```TXT
Задача: написать алгоритм, принимающий на вход строку разделенную пробелами, и длину строки в символах.
Необходимо разбить исходный текст на строки и выравнять по указанной длине строки с помощью пробелов.
Расстояние между словами нужно заполнять равным количеством пробелов, если же это не возможно, то добавляем
еще по пробелу между словами слева направо. Если в строке помещается только 1 слово, то дополнить строку 
пробелами справа Результат вернуть в виде единой строки, где полученный список равных по ширине строк склеен 
с помощью символа перевода строки. 
```