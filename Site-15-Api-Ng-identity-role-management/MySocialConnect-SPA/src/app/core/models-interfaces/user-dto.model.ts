import { PhotoDto } from "./photo-dto.model"

export interface UserDto {
    id: number;
    guId: string;
    userName: string;
    photoUrl: string;
    age: number;
    displayName: string;
    gender: string;
    introduction: string;
    lookingFor: string;
    interests: string;
    city: string;
    country: string;
    photos: PhotoDto[];
    lastActive: Date;
    createdOn: Date;
    updatedOn: Date;
}
