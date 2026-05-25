export enum LogSinkFlag {
    None = 0, // 0
    File = 1 << 0, // 1
    Database = 1 << 1, // 2
    Telegram = 1 << 2, // 4
    Bale = 1 << 3, // 8
    ExternalApi = 1 << 4, // 16
    All = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4),
}
