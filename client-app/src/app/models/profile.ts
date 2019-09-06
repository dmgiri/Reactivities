export interface IProfile {
  displayName: string, 
  username: string,
  bio: string, 
  image: string,
  following: boolean,
  followersCount: number,
  followingCount: number,
  photos: IPhoto[]
}

export interface IProfileFormValues {
  displayName: string,
  bio?: string
}

export interface IPhoto {
  id: string,
  url: string, 
  isMain: boolean
}