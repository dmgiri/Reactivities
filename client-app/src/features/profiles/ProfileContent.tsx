import React from 'react'
import { Tab } from 'semantic-ui-react'
import { IProfile } from '../../app/models/profile'
import ProfilePhotos from './ProfilePhotos'

const panes = [
  { menuItem: 'About', render: () => <Tab.Pane>About content</Tab.Pane>},
  { menuItem: 'Photos', render: () => <ProfilePhotos/>},
  { menuItem: 'Activities', render: () => <Tab.Pane>Activities content</Tab.Pane>},
  { menuItem: 'Followers', render: () => <Tab.Pane>Followers content</Tab.Pane>},
  { menuItem: 'Following', render: () => <Tab.Pane>Following content</Tab.Pane>},
]

interface IProps { profile: IProfile }

const ProfileContent: React.FC<IProps> = ({ profile }) => {
  return (
   <Tab menu={{fluid: true, vertical: true}} menuPosition='right' panes={panes} activeIndex={1} />
  )
}

export default ProfileContent
