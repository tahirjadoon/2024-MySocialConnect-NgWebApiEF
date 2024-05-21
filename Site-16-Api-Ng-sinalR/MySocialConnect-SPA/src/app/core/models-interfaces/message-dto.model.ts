export interface MessageDto {
    id: number;
    guid: string;
    senderId: number;
    senderGuid: string,
    senderUsername: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientGuid: string;
    recipientUsername: string;
    recipientPhotoUrl: string;
    messageContent: string;
    dateMessageRead?: Date;
    dateMessageSent: Date;
}
