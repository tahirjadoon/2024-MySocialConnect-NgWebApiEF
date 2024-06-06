export interface PhotoForApprovalDto {
    id: number;
    url: string;
    userName: string;
    userId: number;
    userGuid: string;
    isApproved: boolean;
}
