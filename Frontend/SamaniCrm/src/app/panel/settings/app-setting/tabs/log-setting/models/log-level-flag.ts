// is enum flag
export enum LogLevelFlag {
  None          = 0,
  Trace         = 1 << 0, // 1
  Debug         = 1 << 1, // 2
  Information   = 1 << 2, // 4
  Warning       = 1 << 3, // 8
  Error         = 1 << 4, // 16
  Critical      = 1 << 5, // 32
  All           = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5), // 63
}




