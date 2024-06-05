export interface PhotoDto {
    id: number;
    url: string;
    isMain: boolean;
    //Added IsApproved with with PhotoManagement and removed PublicId since this is not being used on the client.
    //publicId: string;
    isApproved: boolean;
}
