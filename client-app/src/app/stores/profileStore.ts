import { RootStore } from './rootStore'
import { action, observable, runInAction, computed } from 'mobx'
import agent from '../api/agent'
import { IProfile, IPhoto } from '../models/profile';
import { toast } from 'react-toastify';


export default class ProfileStore 
{
  rootStore: RootStore
  constructor(rootStore: RootStore) { this.rootStore = rootStore }

  @observable profile: IProfile | null = null
  @observable loadingProfile = true
  @observable uploadingPhoto = false
  @observable loading = false

  @computed
  get isCurrentUser() {
    if (this.rootStore.userStore.user && this.profile) { return this.rootStore.userStore.user.username === this.profile.username }
    else return false
  }

  @action
  loadProfile = async (username: string) => {
    this.loadingProfile = true
    try {
      const profile = await agent.Profiles.get(username)
      runInAction(() => { this.profile = profile; this.loadingProfile = false })
    }
    catch (error) { console.log(error); runInAction(() => this.loadingProfile = false) }
  }

  @action
  uploadPhoto = async (file: Blob) => {
    const user = this.rootStore.userStore.user; this.uploadingPhoto = true
    try {
      const photo = await agent.Profiles.uploadPhoto(file)
      runInAction(() => {
        if (this.profile) {
          this.profile.photos.push(photo)
          if (photo.isMain && user) { user.image = photo.url; this.profile.image = photo.url }
          this.uploadingPhoto = false
    }})}
    catch (error) { console.log(error); toast.error('Problem uploading photo'); runInAction(() => this.uploadingPhoto = false) }
  }

  @action
  updateProfile = async (profile: Partial<IProfile>) => {
    const user = this.rootStore.userStore.user
    try {
      await agent.Profiles.updateProfile(profile)
      runInAction(() => {
        if (this.profile && user) {
          if (user.displayName !== profile.displayName) user.displayName = profile.displayName!
          this.profile = { ...this.profile, ...profile}
    }})}
    catch (error) { console.log(error); toast.error('Problem updating profile') }
  }

  @action
  setMainPhoto = async (photo: IPhoto) => {
    const user = this.rootStore.userStore.user; this.loading = true
    try {
      await agent.Profiles.setMainPhoto(photo.id)
      runInAction(() => {
        if (user && this.profile) {
          user.image = photo.url; this.profile.image = photo.url
          this.profile.photos.find(a => a.isMain)!.isMain = false
          this.profile.photos.find(a => a.id === photo.id)!.isMain = true
        };
        this.loading = false
      })
    }
    catch (error) { console.log(error); toast.error('Problem setting main photo'); runInAction(() => this.loading = false) }
  }

  @action
  deletePhoto = async (photo: IPhoto) => {
    this.loading = true
    try {
      await agent.Profiles.deletePhoto(photo.id)
      runInAction(() => {
        if (this.profile) this.profile.photos = this.profile.photos.filter(x => x.id !== photo.id)
        this.loading = false
      })
    }
    catch (error) { console.log(error); toast.error('Problem deleting photo'); runInAction(() => this.loading = false) }
  }
}