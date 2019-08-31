import { observable, computed, action, runInAction } from 'mobx'
import { IUser, IUserFormValues } from '../models/user'
import agent from '../api/agent'
import { RootStore } from './rootStore'
import { history } from '../../index'


export default class UserStore {

  rootStore: RootStore
  constructor(rootStore: RootStore) { this.rootStore = rootStore }

  @observable user: IUser | null = null

  @computed get isLoggedIn() { return !!this.user }

  @action
  login = async (values: IUserFormValues) => {
    try {
      const user = await agent.User.login(values)
      runInAction('logging in', () => { this.user = user })
      this.rootStore.commonStore.setToken(user.token)
      this.rootStore.modalStore.closeModal()
      history.push('/activities')
    }
    catch (error) { throw error }
  }

  @action
  register = async (values: IUserFormValues) => {
    try {
      const user = await agent.User.register(values)
      runInAction('registering', () => { this.user = user })
      this.rootStore.commonStore.setToken(user.token)
      this.rootStore.modalStore.closeModal()
      history.push('/activities')
    }
    catch (error) { throw error }
  }

  @action
  getUser = async () => {
    try { const user = await agent.User.current(); runInAction(() => this.user = user) }
    catch (error) { console.log(error) }
  }

  @action
  logout = () => { this.user = null; this.rootStore.commonStore.token = null; history.push('/') }
}