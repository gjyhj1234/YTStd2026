export interface NavigationItem {
  text: string
  path?: string
  icon?: string
  items?: NavigationItem[]
}

const navigation: NavigationItem[] = [
  {
    text: 'Home',
    path: '/home',
    icon: 'home'
  },
  {
    text: 'Examples',
    icon: 'folder',
    items: [
      {
        text: 'Profile',
        path: '/profile'
      },
      {
        text: 'Tasks',
        path: '/tasks'
      }
    ]
  }
]

export default navigation
