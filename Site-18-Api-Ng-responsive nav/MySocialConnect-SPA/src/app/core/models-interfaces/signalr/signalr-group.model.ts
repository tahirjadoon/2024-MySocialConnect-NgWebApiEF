import { SignalRConnection } from "./signalr-connection.model";

export interface SignalRGroup {
  groupName: string;
  connections: SignalRConnection[];
}
