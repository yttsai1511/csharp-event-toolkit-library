# C# Event Toolkit Library

一個簡單、彈性且強大的 C# 事件管理工具套件，適用於需要動態註冊、取消和調用事件的場景，支援多參數的事件處理。

## 功能特色

- **泛型支援**：靈活處理各類型的委託，支援 `Action` 和 `Func` 的多參數場景。
- **線程安全**：多線程環境下操作安全可靠，避免資源競爭。
- **彈性設計**：動態管理事件，支援特定委託的註冊、移除與合併操作。
- **快速上手**：API 簡單直觀，配合完整範例即刻應用於實際專案中。

## 使用範例

### 1. 註冊與觸發無參數事件

註冊一個無參數事件，用於更新計數器：

```csharp
var toolkit = new EventToolkit();
int counter = 0;

// 註冊無參數事件
toolkit.Register<Action>("UpdateCounter", () => counter++);

// 觸發事件
toolkit.Invoke("UpdateCounter");
toolkit.Invoke("UpdateCounter");

// 現在 counter 的值為 2
```

---

### 2. 註冊與觸發帶參數的事件

註冊一個帶參數的事件，用於記錄使用者操作：

```csharp
var toolkit = new EventToolkit();
var userActions = new List<string>();

// 註冊帶參數事件
toolkit.Register<Action<string>>("TrackUserAction", action => userActions.Add(action));

// 觸發事件
toolkit.Invoke("TrackUserAction", "User clicked button A");
toolkit.Invoke("TrackUserAction", "User selected item B");

// 現在 userActions 包含以下記錄：
// ["User clicked button A", "User selected item B"]
```

---

### 3. 註冊與觸發有返回值的事件

註冊一個有返回值的事件，進行數學運算：

```csharp
var toolkit = new EventToolkit();

// 註冊有返回值的事件
toolkit.Register<Func<int, int, int>>("AddOperation", (a, b) => a + b);
toolkit.Register<Func<int, int, int>>("MultiplyOperation", (a, b) => a * b);

// 觸發事件並收集結果
var results = new List<int>();
toolkit.InvokeFunc("AddOperation", results, 5, 3);  // 5 + 3
toolkit.InvokeFunc("MultiplyOperation", results, 5, 3); // 5 * 3

// 現在 results 包含以下結果：
// [8, 15]
```

---

### 4. 動態管理事件註冊與取消

展示如何註冊、移除特定委託，移除整個事件，並清空所有事件：

```csharp
var toolkit = new EventToolkit();
var logMessages = new List<string>();

// 註冊事件
Action<string> logHandler = message => logMessages.Add(message);
toolkit.Register("LogEvent", logHandler);

// 觸發事件
toolkit.Invoke("LogEvent", "This is a log message");
// logMessages 包含 ["This is a log message"]

// 移除特定委託
toolkit.Unregister("LogEvent", logHandler);

// 目標事件無委託，觸發無效果
toolkit.Invoke("LogEvent", "This will not be logged");

// 再次註冊事件並觸發
toolkit.Register("LogEvent", logHandler);
toolkit.Invoke("LogEvent", "This is another log message");
// logMessages 現在包含 ["This is a log message", "This is another log message"]

// 移除整個事件
toolkit.Unregister("LogEvent");

// 目標事件已移除，觸發無效果
toolkit.Invoke("LogEvent", "This will not be logged either");

// 註冊另一個事件
toolkit.Register<Action>("SampleEvent", () => logMessages.Add("SampleEvent triggered"));

// 清空所有事件
toolkit.Clear();

// 所有事件都已清除，觸發無效果
toolkit.Invoke("SampleEvent"); 
```

## 文件說明

### 事件 API

| 方法名稱 | 功能描述 |
| :----- | :----- |
| `Register<TDelegate>` | 註冊一個新事件與委託 |
| `Unregister<TDelegate>` | 移除特定事件或委託 |
| `Clear` | 清除所有註冊的事件與委託 |
| `Invoke` | 觸發無返回值的事件 |
| `InvokeFunc<TResult>` | 觸發有返回值的事件並收集結果 |

### 主要結構

- `EventKey`：唯一標識每個事件（事件名稱 + 委託型別）。
- `_events`：內部使用的字典，儲存所有註冊的事件及其委託。

## 授權

此專案基於 GPLv3 授權條款，詳情請參閱 LICENSE 文件。